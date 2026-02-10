namespace Employees.Models
{
  public enum EmployeeType
  {
    FullTime,
    PartTime,
    Intern
  }
  
  public class Employee
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public double Salary { get; set; }
    public double Bonus { get; set; }
    public double Tax { get; set; }
    public double Deduction { get; set; }

    public EmployeeType Type { get; set; }

    public double CalculateFinalSalary()
    {

      switch (Type)
      {
        case EmployeeType.FullTime:
      }
      return Type switch
      {
        EmployeeType.FullTime => Salary + Bonus - Tax - Deduction,
        EmployeeType.PartTime => Salary - Tax,
        EmployeeType.Intern => Salary - Deduction,
        _=> throw new InvalidOperationException("Invalid Type")
      };
    }
  }
}