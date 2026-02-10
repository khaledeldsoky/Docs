using MySql.Data.MySqlClient;
using MySql.Data.MySqlClient.Authentication;
using Employees.Models;
using Employees.Services;
using Microsoft.AspNetCore.SignalR;

namespace Employees.Services
{

  public class EmployeeServices
  {

    private readonly string connectionString = "Server=localhost;" + "Port=3306;" + "Database=company_db;" + "User=root;" + "Password=root123;";

    List<Employee> employees = new List<Employee>();

    // public bool  ValidId(int id)
    // {
      
    //   using MySqlConnection connection = new MySqlConnection(connectionString);
    //   connection.Open();

    //   string sql = "SELECT COUNT(*) from employees WHERE id = @id";
    //   using MySqlCommand command = new MySqlCommand(sql, connection);
    //   command.Parameters.AddWithValue("@id", id);

    //   return Convert.ToInt32(command.ExecuteScalar()) > 0;
    // }

    public List<Employee> GetAll()
    {
      employees.Clear();

using MySqlConnection connection = new MySqlConnection(connectionString);
connection.Open();

      string sql = "Select * from employees";
      using MySqlCommand command = new MySqlCommand(sql, connection);

      using MySqlDataReader dataReader = command.ExecuteReader();

      while (dataReader.Read())
      {
        Employee employee = new Employee();

        employee.Name = dataReader.GetString("name");
        employee.Salary = dataReader.GetInt32("salary");
        employee.Type = (EmployeeType)dataReader.GetInt32("type");

        employees.Add(employee);
      }

      return employees;
    }

    public void Add(Employee employee)
    {
      
      if (string.IsNullOrWhiteSpace(employee.Name))
        throw new ArgumentException("The name is required");

      if (employee.Salary < 0)
        throw new ArgumentException("Salary must be positive");
        
      using MySqlConnection connection = new MySqlConnection(connectionString);
      connection.Open();

      string sql = "insert into  employees (name , salary , tax  ,bonus , deduction , type)"
                  + "Values(@n,@s,@tax,@b,@d,@t)";
      using MySqlCommand command = new MySqlCommand(sql, connection);

      command.Parameters.AddWithValue("@n", employee.Name);
      command.Parameters.AddWithValue("@s", employee.Salary);
      command.Parameters.AddWithValue("@tax", employee.Tax);
      command.Parameters.AddWithValue("@b", employee.Bonus);
      command.Parameters.AddWithValue("@d", employee.Deduction);
      command.Parameters.AddWithValue("@t", employee.Type);

      command.ExecuteNonQuery();
    }

    public bool Delete(int id)
    {
      using MySqlConnection connection = new MySqlConnection(connectionString);
      connection.Open();

      string sql = "Delete from employees where id = @id";
      using MySqlCommand command = new MySqlCommand(sql, connection);

      command.Parameters.AddWithValue("@id", id);

      command.ExecuteNonQuery();

      return command.ExecuteNonQuery() > 0;
    }

    public Employee GetById(int id)
    {

      // if (!ValidId(id))
      //   return null;

      Employee employee = null;
      using MySqlConnection connection = new MySqlConnection(connectionString);
      connection.Open();

      string sql = "Select * from employees where id = @id";
      using MySqlCommand command = new MySqlCommand(sql, connection);

      command.Parameters.AddWithValue("@id", id);
      command.ExecuteNonQuery();

      using MySqlDataReader dataReader = command.ExecuteReader();

      if(!dataReader.Read())
        return null;

      while (dataReader.Read())
      {
        employee = new Employee();
        employee.Name = dataReader.GetString("name");
        employee.Salary = dataReader.GetInt32("salary");
        employee.Type = (EmployeeType)dataReader.GetInt32("type");
      }

      return employee;
    }

  }
}