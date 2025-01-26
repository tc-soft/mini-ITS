![Version](https://img.shields.io/badge/version-1.0.0-blue)
![License: MIT](https://img.shields.io/badge/license-MIT-green)
![Backend](https://img.shields.io/badge/backend-.NET%209.x-blueviolet)
![React](https://img.shields.io/badge/frontend-React-blue)

<p align="left">
  <img src="https://raw.githubusercontent.com/tc-soft/mini-ITS/main/mini-ITS.Web/ClientApp/src/images/mini-ITS.svg" alt="Logo mini-ITS" width="150">
</p>

### mini-ITS - a small reporting system to your comany.

### <span style="color:blue">**RestAPI of Users**</span>
*route : api/Users*

| Resource / HTTP method | POST (create)      | GET (read)       | PUT (update)       | PATCH           | DELETE (delete)   |
| ---------------------- | ------------------ | ---------------- | ------------------ | --------------- |------------------ |
| /Login                 | Login user         |                  |                    |                 |                   |
| /Logout                |                    |                  |                    |                 | Logout user       |
| /LoginStatus           |                    | Check status     |                    |                 |                   |
| /Index                 |                    | List users       |                    |                 |                   |
| /Create                | Create user        |                  |                    |                 |                   |
| /Edit/{id}             |                    | Edit user        | Update user        |                 |                   |
| /Delete/{id}           |                    |                  |                    |                 | Delete user       |
| /ChangePassword        |                    |                  |                    | Change password |                   |

### <span style="color:blue">**RestAPI of Groups**</span>
*route : api/Groups*

| Resource / HTTP method | POST (create)      | GET (read)       | PUT (update)       | PATCH           | DELETE (delete)   |
| ---------------------- | ------------------ | ---------------- | ------------------ | --------------- |------------------ |
| /Index                 |                    | List groups      |                    |                 |                   |
| /Create                | Create group       |                  |                    |                 |                   |
| /Edit/{id}             |                    | Edit user        | Update user        |                 |                   |
| /Delete/{id}           |                    |                  |                    |                 | Delete group      |

### <span style="color:blue">**RestAPI of Enrollments**</span>
*route : api/Enrollments*

| Resource / HTTP method | POST (create)      | GET (read)       | PUT (update)       | PATCH           | DELETE (delete)   |
| ---------------------- | ------------------ | ---------------- | ------------------ | --------------- |------------------ |
| /Index                 |                    | List enrollments |                    |                 |                   |
| /Create                | Create enrollment  |                  |                    |                 |                   |
| /Edit/{id}             |                    | Edit enrollment  | Update enrollment  |                 |                   |
| /Delete/{id}           |                    |                  |                    |                 | Delete enrollment |

### <span style="color:blue">**RestAPI of EnrollmentsDescription**</span>
*route : api/EnrollmentsDescription*

| Resource / HTTP method | POST (create)                     | GET (read)                                          | PUT (update)                      | PATCH | DELETE (delete)                   |
| ---------------------- | --------------------------------- | --------------------------------------------------- | --------------------------------- | ----- |---------------------------------- |
| /Index                 |                                   | List<br> enrollmentsDescription                     |                                   |       |                                   |
| /Index/{id}            |                                   | List<br> enrollmentsDescription<br> by EnrollmentId |                                   |       |                                   |
| /Create                | Create<br> enrollmentsDescription |                                                     |                                   |       |                                   |
| /Edit/{id}             |                                   | Edit<br> enrollmentsDescription                     | Update<br> enrollmentsDescription |       |                                   |
| /Delete/{id}           |                                   |                                                     |                                   |       | Delete<br> enrollmentsDescription |

### <span style="color:blue">**RestAPI of EnrollmentsPicture**</span>
*route : api/EnrollmentsPicture*

| Resource / HTTP method | POST (create)                     | GET (read)                                          | PUT (update)                      | PATCH | DELETE (delete)                   |
| ---------------------- | --------------------------------- | --------------------------------------------------- | --------------------------------- | ----- |---------------------------------- |
| /Index/{id}            |                                   | List<br> enrollmentsPicture<br> by EnrollmentId     |                                   |       |                                   |
| /Create                | Create<br> enrollmentsPicture     |                                                     |                                   |       |                                   |
| /Edit/{id}             |                                   | Edit<br> enrollmentsPicture                         | Update<br> enrollmentsPicture     |       |                                   |
| /Delete/{id}           |                                   |                                                     |                                   |       | Delete<br> enrollmentsPicture     |


## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change. Please make sure to update tests as appropriate.

Tadeusz Ciszewski
office@tc-soft.pl