# mini-ITS
mini-ITS - a small reporting system to your comany.

### <span style="color:blue">**RestAPI of Users**</span>
*route : api/Users*

| Resource / HTTP method | POST (create) | GET (read)   | PUT (update) | PATCH           | DELETE (delete) |
| ---------------------- | ------------- | ------------ | ------------ | --------------- |---------------- |
| /Login                 | Login user    |              |              |                 |                 |
| /Logout                |               |              |              |                 | Logout user     |
| /LoginStatus           |               | Check status |              |                 |                 |
| /Index                 |               | List users   |              |                 |                 |
| /Create                | Create user   |              |              |                 |                 |
| /Edit/{id}             |               | Edit user    | Update user  |                 |                 |
| /Delete/{id}           |               |              |              |                 |                 |
| /ChangePassword        |               |              |              | Change password |                 |

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change. Please make sure to update tests as appropriate.

Tadeusz Ciszewski
office@tc-soft.pl