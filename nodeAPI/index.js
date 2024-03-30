const http = require('http')
const exp = require('express')
const app = exp()
const bparse = require('body-parser')
const url = require('url')
const mysql = require('mysql')
// const { restart } = require('nodemon')
// const { query } = require('express')
// const { strictEqual } = require('assert')
// const { rmSync } = require('fs')
//const path = require('path')
const swaggerUI = require("swagger-ui-express");
const swaggerDocument = require('./openapi.json');
const swaggerJsDoc = require("swagger-jsdoc");

app.use(bparse.json())
app.use(bparse.urlencoded({ extended: true }))


const options = {
    definition: {
      openapi: "3.0.0",
      info: {
        title: 'indexed.js',
        version: "1.0.0",
        description: "Simple restful booking CRUD",
      },
      servers: [
        {
          url: `http://127.0.0.1:3000`,
        },
      ],
    },
    apis: ["./*.js"],
  }
  const specs = swaggerJsDoc(options)
  app.get("/openapi.json", (req, res) => res.json(swaggerDocument))
  app.use("/docs", swaggerUI.serve, swaggerUI.setup(swaggerDocument))

/************ SQL CONNECTION **************/ 
var dbName = 'employees'
var conn = mysql.createConnection({
    host     : 'localhost',
    user     : 'root',
    password : '',
    database : dbName
  });

  conn.connect(function(err) {
    if (err) throw err
    console.log("Successfully connected to %s DB!", dbName)
  })


/******** SERVER PORT ************/
const server = app.listen(3000, "127.0.0.1", function () {
    var host = server.address().address
    var port = server.address().port

    console.log("Server started at URI: http://%s:%s", host, port)
})

/************* GATEWAY TO PHP API ************/
//          for WinForm login system
app.get("/api", function (req, res) {
    var q = url.parse(req.url, true).query
    var username = q.username
    var password = q.password
    var mypath = '/les/SZOP_Beadando/login.php?username=' + username + '&password=' + password
    

    var apiReq = http.request({
        hostname: "127.0.0.1",
        path: mypath,
        method: 'GET'
    },
        function (apiRes) {
            apiRes.on('data', d => {
                // console.log(d)
                var data = JSON.parse(d)
                //data.id = changeId
                res.json(data)
            })
        })
    apiReq.on('error', error => {
        console.log(error)
        res.status(504).send({
            'error': 1,
            'message': "Internal server error. API could not get a response in time."
        })
    })
    apiReq.end()
})


// //POST
// //query pelda nodejs-mysql
// app.post("/api", function (req, res) {
//     var username = req.body.username
//     var password = req.body.password

//     // var querystr = "select * from users;" 

//     var querystr = "select * from users where username ='" + username + "' and password = md5('" + password + "');" 

//     var query = conn.query(querystr, function (err, results) {
//         if(err) { throw err }

//         res.json({
//             'error': 0,
//             'message': "Success!",
//             'user': results[0]
//         })
//       })

// })


/************** CRUD COUNTER ****************/
var changeid = 0;
app.get("/api/changeid", function(req, res){
    res.json({
        'error': 0,
        'message': "Change id",
        'changeid': changeid
    })
})

function inc_changeid() {
    changeid ++;
}


/*************** LOGIN ******************/
// app.get("/api/login", function(req, res){
//     var q = url.parse(req.url, true).query
//     var username = q.username
//     var password = q.password

//     if (!username && !password){
//         res.json({
//             'error': 1,
//             'message': "Missing username or password!",
//         })
//         return
//     }

//     var querystr = "select * from users where username ='" + username + "' and password = md5('" + password + "');"
//     var query = conn.query(querystr, function (err, results) {
//         if(err) { throw err }

//         if (results.length == 0){
//             res.json({
//                 'error': 1,
//                 'message': "Incorrect username or password!"
//             })
//         }
//         else{
//             res.json({
//                 'error': 0,
//                 'message': "Successfully logged in!"
//             })
//         }
//       })
// })


/*************** BOOKINGS CRUD ******************/
//GET
app.get('/api/bookings', function (req, res) {
    var querystr = "select * from bookings;"
    var query = conn.query(querystr, function (err, results) {
        if(err) { 
            throw err 
        }
        if (results.length == 0){
            res.json({
                'error': 1,
                'message': "Bookings read error!"
            })
        }
        else{
            res.json({
                'error': 0,
                'message': "Successfully read bookings!",
                'bookings': results
            })
        }
      })  
})

//GET by id
app.get('/api/bookings/id/:id', function (req, res) {
    var querystr = "select * from bookings where id=?;"
    var query = conn.query(querystr, [req.params.id], function (err, results) {
        if(err) { 
            throw err 
        }
        if (results.length == 0){
            res.json({
                'error': 1,
                'message': "Id " + req.params.id + " doesn't exist!"
            })
        }
        else{
            res.json({
                'error': 0,
                'message': "Successfully read bookings!",
                'bookings': results
            })
        }
      })  
})

//GET by name
app.get('/api/bookings/name/:name', function (req, res) {
    var name = req.params.name

    var querystr = "select * from bookings where name like '%" + name + "%';"

    var query = conn.query(querystr, [req.params.name], function (err, results) {
        if(err) { 
            throw err 
        }
        if (results.length == 0){
            res.json({
                'error': 1,
                'message': "Name " + name + " not in table!"
            })
        }
        else{
            res.json({
                'error': 0,
                'message': "Successfully read bookings!",
                'bookings': results
            })
        }
      })  
})

//GET by date
app.get('/api/bookings/bookingDate/:bookingDate', function (req, res) {
    var date = req.params.bookingDate

    var querystr = "select * from bookings where bookingDate like '%" + date + "%';"

    var query = conn.query(querystr, [req.params.name], function (err, results) {
        if(err) { 
            throw err 
        }
        if (results.length == 0){
            res.json({
                'error': 1,
                'message': "Date " + date + " not in table!"
            })
        }
        else{
            res.json({
                'error': 0,
                'message': "Successfully read bookings!",
                'bookings': results
            })
        }
      })  
})

//DELETE by id
app.delete('/api/bookings/:id', function (req, res) {
    var q = url.parse(req.url, true).query
    var username = q.username
    var password = q.password
    var id = req.params.id

    if (!username && !password){
        res.json({
            'error': 1,
            'message': "Missing username or password!",
        })
        return
    }

	var querystr = "select count(*) as cnt from users where username='" + username + "' and password=md5('" + password + "');"
	var query = conn.query(querystr, (err, results) => {
		if(err) throw err;
		if (results[0].cnt == 0) {
			return res.json({
                'error': 1, 
                'message': 'Log in to delete from table!'
            });
		}
		else {
			if (!id) {
				return res.json({
                    'error': 1, 
                    'message': 'ID param is missing!'});
			}	
			querystr = "delete from bookings where id=" + id + ";";	
			query = conn.query(querystr, (err, results) => {
				if(err) throw err;
				inc_changeid();

				return res.json({
                    'error': 0, 
                    'message': "Id " + id + " has been deleted!"
                });
			}
		)}
	});
});

//INSERT
app.post('/api/bookings', function (req, res) {
	var username = req.body.username;
	var password = req.body.password;
    var name = req.body.name;
    var phone = req.body.phone;
    var email = req.body.email;
    var bookingDate = req.body.bookingDate;

    if (!email || email == null || email == "" || email == " "){
        email = "";
    }

	if (!username || !password){
        res.json({
            'error': 1,
            'message': "Missing username or password!",
        })
        return
    }

    var querystr = "select count(*) as cnt from users where username='" + username + "' and password=md5('" + password + "');"
	var query = conn.query(querystr, (err, results) => {
		if(err) throw err;
		if (results[0].cnt == 0) {
			return res.json({
                'error': 1, 
                'message': 'Log in to insert into table!'
            });
		}
		else {
            if (!name || !phone || !bookingDate) {
				return res.json({
                    'error': 1, 
                    'message': 'You have missing or null params!'});
			}	
            querystr = "insert into bookings (name, phone, email, bookingDate) values ('" + name + "', '" + 
                        phone + "', '" + email + "', '" + bookingDate + "');"
			query = conn.query(querystr, (err, results) => {
				if(err) throw err;
                inc_changeid()

				return res.json({
                    'error': 0, 
                    'message': "New record inserted for " + bookingDate + "!"
                })
            })
        }
    })
})

//UPDATE by id
app.put('/api/bookings', function (req, res) {
	var username = req.body.username;
	var password = req.body.password;
    var id = req.body.id;
    var name = req.body.name;
    var phone = req.body.phone;
    var email = req.body.email;
    var bookingDate = req.body.bookingDate;

    if (!username || !password){
        res.json({
            'error': 1,
            'message': "Missing username or password!",
        })
        return
    }

    var querystr = "select count(*) as cnt from users where username='" + username + "' and password=md5('" + password + "');"
	var query = conn.query(querystr, (err, results) => {
		if(err) throw err;
		if (results[0].cnt == 0) {
			return res.json({
                'error': 1, 
                'message': 'Log in to update table!'
            });
		}
		else {
            if (!id || !name || !phone || !bookingDate) {
                return res.json({
                    'error': 1, 
                    'message': 'You have missing or null params!'});
			}	
            querystr = "update bookings set name='" + name + "', phone='" + phone + "', email='" + 
                        email + "', bookingDate='" + bookingDate + "' where id=" + id + ";"
			query = conn.query(querystr, (err, results) => {
				if(err) throw err
                inc_changeid()

				return res.json({
                    'error': 0, 
                    'message': "Successfully updated booking id " + id + "!"
                });
            })
        }
    })
})


/***************** USERS CRUD : ADMIN ******************/
//GET
app.get('/api/users', function (req, res) {
    var querystr = "select * from users;"
    var query = conn.query(querystr, function (err, results) {
        if(err) { 
            throw err 
        }
        if (results.length == 0){
            res.json({
                'error': 1,
                'message': "Users read error!"
            })
        }
        else{
            res.json({
                'error': 0,
                'message': "Successfully read users!",
                'users': results
            })
        }
      })  
})

//GET by id
app.get('/api/users/id/:id', function (req, res) {
    var querystr = "select * from users where id=?;"
    var query = conn.query(querystr, [req.params.id], function (err, results) {
        if(err) { 
            throw err 
        }
        if (results.length == 0){
            res.json({
                'error': 1,
                'message': "Id " + req.params.id + " doesn't exist!"
            })
        }
        else{
            res.json({
                'error': 0,
                'message': "Successfully read users!",
                'users': results
            })
        }
      })  
})

//GET by name
app.get('/api/users/user/:user', function (req, res) {
    var user = req.params.user

    var querystr = "select * from users where username like '%" + user + "%';"

    var query = conn.query(querystr, [req.params.user], function (err, results) {
        if(err) { 
            throw err 
        }
        if (results.length == 0){
            res.json({
                'error': 1,
                'message': "Name " + user + " not in table!"
            })
        }
        else{
            res.json({
                'error': 0,
                'message': "Successfully read users!",
                'users': results
            })
        }
      })  
})

//GET by isAdmin
app.get('/api/users/isAdmin/:isAdmin', function (req, res) {
    var querystr = "select * from users where isAdmin=?;"
    var query = conn.query(querystr, [req.params.isAdmin], function (err, results) {
        if(err) { 
            throw err 
        }
        if (results.length == 0){
            res.json({
                'error': 1,
                'message': "No results found!"
            })
        }
        else{
            res.json({
                'error': 0,
                'message': "Successfully read users!",
                'users': results
            })
        }
      })  
})

//DELETE by id
app.delete('/api/users/:id', function (req, res) {
    var q = url.parse(req.url, true).query
    var username = q.username
    var password = q.password
    var id = req.params.id

    if (!username && !password){
        res.json({
            'error': 1,
            'message': "Missing username or password!",
        })
        return
    }

	var querystr = "select count(*) as cnt from users where username='" + username + "' and password=md5('" + password + "') and isAdmin=1;"
	var query = conn.query(querystr, (err, results) => {
		if(err) throw err;
		if (results[0].cnt == 0) {
			return res.json({
                'error': 1, 
                'message': 'Log in as an admin to use this function!'
            });
		}
		else {
			if (!id) {
				return res.json({
                    'error': 1, 
                    'message': 'ID param is missing!'});
			}	
			querystr = "delete from users where id=" + id + ";";	
			query = conn.query(querystr, (err, results) => {
				if(err) throw err
				inc_changeid()

				return res.json({
                    'error': 0, 
                    'message': "Id " + id + " has been deleted!"
                })
			}
		)}
	});
});

//INSERT
app.post('/api/users', function (req, res) {
	var username = req.body.username;
	var password = req.body.password;
    var user = req.body.user;
    var pwd = req.body.pwd;
    var isAdmin = req.body.isAdmin;

	if (!username || !password){
        res.json({
            'error': 1,
            'message': "Missing username or password!",
        })
        return
    }

    var querystr = "select count(*) as cnt from users where username='" + user + "';"
    var query = conn.query(querystr, (err, results) => {
        if (err) throw err
        if (results[0].cnt != 0) {
            return res.json({
                'error': 1,
                'message': "Username " + user + " is not unique!"
            })
        }
        else{
            var querystr = "select count(*) as cnt from users where username='" + username + "' and password=md5('" + password + "') and isAdmin=1;"
            var query = conn.query(querystr, (err, results) => {
                if(err) throw err
                if (results[0].cnt == 0) {
                    return res.json({
                        'error': 1, 
                        'message': 'Log in as an admin to use this function!'
                    });
                }
                else {
                    if (!user || !pwd || isAdmin == null) {
                        return res.json({
                            'error': 1, 
                            'message': 'You have missing or null params!'});
                    }	
                    querystr = "insert into users (username, password, isAdmin) values ('" + user + "', md5('" + 
                                pwd + "'), '" + isAdmin + "');"
                    query = conn.query(querystr, (err, results) => {
                        if(err) throw err;
                        inc_changeid()
        
                        return res.json({
                            'error': 0, 
                            'message': "New record " + user + " inserted!"
                        })
                    })
                }
            })
        }
    })  
})

//UPDATE by id
app.put('/api/users', function (req, res) {
	var username = req.body.username;
	var password = req.body.password;
    var id = req.body.id;
    var user = req.body.user;
    var pwd = req.body.pwd;
    var isAdmin = req.body.isAdmin;

    if (!username || !password){
        res.json({
            'error': 1,
            'message': "Missing username or password!",
        })
        return
    }

    var querystr = "select count(*) as cnt from users where username='" + username + "' and password=md5('" + password + "') and isAdmin=1;"
	var query = conn.query(querystr, (err, results) => {
		if(err) throw err;
		if (results[0].cnt == 0) {
			return res.json({
                'error': 1, 
                'message': 'Log in as an admin to use this function!'
            });
		}
		else {
            if (!id || !user || isAdmin == null) {
                return res.json({
                    'error': 1, 
                    'message': 'You have missing or null params!'});
			}

            querystr = "update users set username='" + user + "', password=md5('" + pwd + "'), isAdmin='" + 
                isAdmin + "' where id=" + id + ";"
            // if (pwd == "12345") {
            //     querystr = "update users set username='" + user + "', password=md5('" + pwd + "'), isAdmin='" + 
            //     isAdmin + "' where id=" + id + ";"
            // }
            // else{
            //     querystr = "update users set username='" + user + "', isAdmin='" + 
            //     isAdmin + "' where id=" + id + ";"
            // }
             
			query = conn.query(querystr, (err, results) => {
				if(err) throw err
                inc_changeid()

				return res.json({
                    'error': 0, 
                    'message': "Successfully updated user " + id + "!"
                });
            })
        }
    })
})

/***************** GET USER ID ******************/
//GET logged in id
app.get('/api/usersid', function (req, res) {
    var q = url.parse(req.url, true).query
    var username = q.username
    var password = q.password

    if (!username && !password){
        res.json({
            'error': 1,
            'message': "Missing username or password!",
        })
        return
    }

    var querystr = "select count(*) as cnt from users where username='" + username + "' and password=md5('" + password + "');"
	var query = conn.query(querystr, (err, results) => {
		if(err) throw err;
		if (results[0].cnt == 0) {
			return res.json({
                'error': 1, 
                'message': 'Log in to get logged in Id'
            });
		}
		else {
            querystr = "select id from users where username='" + username + "' and password=md5('" + password + "');"
	        query = conn.query(querystr, (err, results) => {
		        if(err) throw err;
                return res.json({
                    'error': 0, 
                    'message': 'Id retrieved successfully!',
                    'id': results[0].id
                })               
            })
        }
    })
})


/***************** USER CRUD: USERS ******************/
//GET
app.get('/api/user', function (req, res) {
    var q = url.parse(req.url, true).query
    var id = q.id

    var querystr = "select * from users where id=" + id + ";"
    var query = conn.query(querystr, function (err, results) {
        if(err) { 
            throw err 
        }
        if (results.length == 0){
            res.json({
                'error': 1,
                'message': "Users read error!"
            })
        }
        else{
            res.json({
                'error': 0,
                'message': "Successfully read users!",
                'users': results
            })
        }
      })  
})

//UPDATE by id
app.put('/api/user', function (req, res) {
	var username = req.body.username;
	var password = req.body.password;
    var user = req.body.user;
	var pwd = req.body.pwd;
    var id = req.body.id;

    if (!user || user == null || user == "" || user == " "){
        user = username
    }
    if (!pwd || pwd == null || pwd == "" || pwd == " "){
        pwd = password
    }

    if (!username || !password){
        res.json({
            'error': 1,
            'message': "Missing username or password!",
        })
        return
    }

    var querystr = "select count(*) as cnt from users where username='" + username + "' and password=md5('" + password + "');"
	var query = conn.query(querystr, (err, results) => {
		if(err) throw err;
		if (results[0].cnt == 0) {
			return res.json({
                'error': 1, 
                'message': 'Log in to use this feature!'
            });
		}
		else {
            if (!id || !user || !pwd) {
                return res.json({
                    'error': 1, 
                    'message': 'You have null params!'});
			}	
            querystr = "update users set username='" + user + "', password=md5('" + pwd + "') where id=" + id + ";"
			query = conn.query(querystr, (err, results) => {
				if(err) throw err
                inc_changeid()

				return res.json({
                    'error': 0, 
                    'message': "Successfully updated user " + id + "!"
                });
            })
        }
    })
})

/***************** GET CHANGE COUNT ******************/
app.get('/api/cnt', function (req, res) {
    var q = url.parse(req.url, true).query
    var username = q.username
    var password = q.password

    if (!username && !password){
        res.json({
            'error': 1,
            'message': "Missing username or password!",
        })
        return
    }

    var querystr = "select count(*) as cnt from users where username='" + username + "' and password=md5('" + password + "');"
	var query = conn.query(querystr, (err, results) => {
		if(err) throw err;
		if (results[0].cnt == 0) {
			return res.json({
                'error': 1, 
                'message': 'Log in to get logged in Id'
            });
		}
		else {
            return res.json({
                'error': 0, 
                'message': 'Change count sent!',
                'id': changeid
            })
        }
    })
})


