CREATE TABLE users (
    id INT AUTO_INCREMENT PRIMARY KEY,
    username VARCHAR(100) NOT NULL ,
    email VARCHAR(255) NOT NULL UNIQUE,
    password VARCHAR(255) NOT NULL,
    phone VARCHAR(100) NOT NULL
);

Select password from users where email = 'mohamed@gmail.com'

select count(*) from users where email = 'khaled@gmail.com'

SELECT * FROM users

UPDATE users SET password = "" WHERE id = 1

DELETE users  FROM users WHERE password = '$2a$11$KnSyzb5U9hzLKLFharGq7uDee0t7jDoF7OasO9tFDaBrnw8UsuIgK'