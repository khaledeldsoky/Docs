# Create a new console app
  dotnet new console -o MyFirstApp 
  dotnet run

# Create a new Web app
dotnet new webapi -n EmployeeApi
dotnet run

swagger

# --------------------
dotnet add package MySql.Data
dotnet add package BCrypt.Net-Next


delete the last commint local

git reset HEAD~1


delete the last two commint local

git reset --soft HEAD~2

static : we call it directly with class name
no static:if we dont use static we should make object from class to use it 

foreach : we use to itrate for all the list (we dont specif the start or the end , it itrate from fist to end)
for : it iterate from what i choose and to what i choose 
while: we don't know the itration

sync : متزامن
async : غير متزامن 

void : don't rertun

inheretance
private method or string : you can't call it in the inheretance 
protect method or string : you can call it in the inheretance 
interal : you can see it the same project

abstract    : we can't make object but can inhertance 
class       : we can make object and inhertance