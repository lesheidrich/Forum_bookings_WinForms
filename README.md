# Forum Bookings API
Forum Night Club is an imaginary venue that books live music events. The program allows Forum employees to log in to a system to perform basic CRUD operations in an events database, where they can manage bookings based on dates. Users also have access to their own user data for username and password management. Admin users can perform CRUD operaitons with employee data, but do not have access to bookings data.<br>
<br>
The login system utilizes a Node JS gateway to a PHP API, which communicates with the C# WinForm client through RESTful URIs.<br>
Once logged in, users and admins utilize Node RESTful URI's through the aforementioned WinForm client.<br>
Node API tracks changes through a CRUD counter. If the server and client counter do not match, the system autor refreshes to ensure data integrity.nBoth tables support character string based searching for their respective name fields, and ID based searching. The bookings table also supports date based searching.<br>
<br>
Setup:
<ul>
  <li>download all contents of PHP API: SZOP_Beadando -> xampp/htdocs/les/SZOP_Beadando</li>
  <li>download WinFormClient -> your_username_here/source/repos/WinFormClient</li>
  <li>run XAMPP (Apacse + MySQL server)</li>
  <li>download nodeAPI to anywhere</li>
  <li>run command line and cd into nodeAPI</li>
  <li>npm install dependencies</li>
  <li>run: node index.js</li>
  <li>run WinFormClient</li>
  <li>Swagger OpenAPI: http://127.0.0.1:3000/docs</li>
</ul>

The program was created for EKKE's Computer Science BSC - Service Oriented Programming course in 2022.
<br><br>
<img src="https://github.com/CoGn151oN/forum_bookings_api/blob/main/demo_img/login.PNG?raw=true">
<br><br>
<img src="https://github.com/CoGn151oN/forum_bookings_api/blob/main/demo_img/users.PNG?raw=true">
<br><br>
<img src="https://github.com/CoGn151oN/forum_bookings_api/blob/main/demo_img/admin.PNG?raw=true">
