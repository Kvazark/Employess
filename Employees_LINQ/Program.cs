using System;
using System.Linq;
using System.Text;
using Employees_DatabaseFirst.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Employees_DatabaseFirst
{
    class Program
    {
        static private EmployeesContext _context = new EmployeesContext();
        static void Main(string[] args)
        {
            //Console.WriteLine(GetEmployeesInformation());
           // Console.WriteLine(Task1());
          // Console.WriteLine(Task2());
           //Console.WriteLine(Task3());
           Console.WriteLine(Task4());
           //Console.WriteLine(Task5());
          // Console.WriteLine(Task6());
          // Task7();
           //Task8();
        }

        static string GetEmployeesInformation()
        {
            var employees = from e in _context.Employees orderby e.EmployeeId select e;
            var sb = new StringBuilder();
            int k = 0;
            foreach (var e in employees)
            {
                sb.AppendLine($"{k++} {e.FirstName} {e.LastName} {e.MiddleName} {e.JobTitle}");
            }

            return sb.ToString().TrimEnd();
        }

        //Вывод сотрудников, чья зарплата больше 48к
        static string Task1()
        {
            var employees = from e in _context.Employees orderby e.Salary where e.Salary > 48000 select e;
            var sb = new StringBuilder();
            //Console.ForegroundColor = ConsoleColor.Blue;
            //Console.WriteLine("FirstName|LastName|MiddleName|JobTitle|DepartmentId|ManagerId|HireDate|Salary|AddressId");
            //Console.ResetColor();
            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} {e.MiddleName} {e.JobTitle} {e.DepartmentId} {e.ManagerId} {e.HireDate} {e.Salary} {e.AddressId}");
            }

            return sb.ToString().TrimEnd();
        }

        //Задание о переселении сотрудника на новый адрес
        static string Task2()
        {
            
            var adressBrown = new Address()
            {
                AddressText = "Mathias Brown Street"
            };
            _context.Addresses.Add(adressBrown);
            _context.SaveChanges();

            var employees = (from e in _context.Employees where e.LastName.Equals("Brown") select e).ToList();
            
            var sb = new StringBuilder();
            foreach (var e in employees)
            {
                e.Address = adressBrown;
                _context.SaveChanges();
                sb.AppendLine($"{e.FirstName} {e.LastName} {e.MiddleName} {e.JobTitle} {e.DepartmentId} {e.ManagerId} {e.HireDate} {e.Salary} {e.AddressId}");
            }
            return sb.ToString().TrimEnd();
        }

        //Аудит проектов
        static string Task3()
        {
            var specificProjects = (from ep in _context.EmployeesProjects
                join e in _context.Employees on ep.EmployeeId equals e.EmployeeId
                where ep.Project.StartDate.Year >= 2002 && ep.Project.StartDate.Year <= 2005
                select new
                {
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Manager = e.Manager,
                    ProjectName = ep.Project.Name,
                    StartDate = ep.Project.StartDate,
                    EndDate = ep.Project.EndDate
                }).Take(5).ToList();

            var sb = new StringBuilder();
            
            foreach (var sp in specificProjects)
            {
                if (sp.EndDate == null)
                {
                    sb.AppendLine($"Сотрудник: {sp.FirstName} {sp.LastName} Менеджер: {sp.Manager.FirstName} {sp.Manager.LastName}  \n Проект: {sp.ProjectName} начался {sp.StartDate}, не заверщён.\n");
                }
                else
                {
                    sb.AppendLine($"Сотрудник: {sp.FirstName} {sp.LastName} Менеджер: {sp.Manager.FirstName} {sp.Manager.LastName}  \n Проект: {sp.ProjectName} начался {sp.StartDate}, завершился {sp.EndDate}.\n");
                }
            }
            return sb.ToString().TrimEnd();
        }
        
        //Досье на сотрудника
        static string Task4()
        {
            int id = Convert.ToInt32(Console.ReadLine());
            var employee =( from e in _context.Employees
            join ep in _context.EmployeesProjects on  e.EmployeeId equals ep.EmployeeId
            where e.EmployeeId == id
            select new
            {
                EmployeeId = e.EmployeeId,
                FirstName = e.FirstName, 
                LastName = e.LastName, 
                MiddleName = e.MiddleName,
                JobTitle = e.JobTitle, 
                ProjectName = ep.Project.Name
            }).ToList();
            var sb = new StringBuilder();
            
            sb.AppendLine($"{employee[0].FirstName} {employee[0].LastName} {employee[0].MiddleName} - {employee[0].JobTitle}\nПроекты: ");
            
            foreach (var e in employee)
            {
                sb.AppendLine($" {e.ProjectName}");
            }
            return sb.ToString().TrimEnd();
        }

        //Вывод названий отделов, где менее 5 сотрудников
        static string Task5()
        {
            var departaments = (from d in  _context.Departments
                where d.Employees.Count()<5
                select d) .ToList();
            var sb = new StringBuilder();
            Console.WriteLine("Название отделов, где менее 5 сотрудников: ");
            foreach (var d in departaments)
            {
                sb.AppendLine($" {d.Name}");
            }
            return sb.ToString().TrimEnd();
        }
        
        //Увеличение зарплаты отделу на X%
        static string Task6()
        {
            Console.Write("Введите название отдела: ");
            string name = Console.ReadLine();
            var departament = (from d in _context.Departments
                where d.Name == name
                select d ).First();
            Console.Write("Введите на сколько процентов нужно повысить зарплату: ");
            int percent = int.Parse(Console.ReadLine());
            var employees =(from e in _context.Employees
                where e.Department.Name == name
                select e).ToList();
            
            var sb = new StringBuilder();
            foreach (var e in employees)
            {
                var lastSalary = e.Salary;
                e.Salary*= (decimal)(1+percent/100f);
                sb.AppendLine($"{e.FirstName} {e.LastName}: прошлая зарплата - {lastSalary}, повышенная зарплата {e.Salary}");
            }
            _context.SaveChanges();
            return sb.ToString().TrimEnd();
        }
        
        //Удаление отдела с заданным id
        static void Task7()
        {
            Console.Write("Введите id отдела для его удаления: ");
            int id = Convert.ToInt32(Console.ReadLine());
            var department = (
                from d in _context.Departments
                where d.DepartmentId == id
                select d
            ).First();

            if (department == null)
            {
                Console.WriteLine("Отдел не найден.");
                return;
            }

            department.Employees.Clear();
            _context.SaveChanges();  
        }
        
        //Удаление города с заданным названием
        static void Task8()
        {
            Console.Write("Введите название города для его удаления: ");
            string townName = Console.ReadLine();
            var deletTown = (from t in _context.Towns where t.Name == townName select t).First();
            
            Address adress = new Address()
            {
                AddressText = "is unknown"
            };
            var deletAdress = (from a in _context.Addresses where a.Town == deletTown select a).ToList();
            var employees = (from e in _context.Employees
                where e.Address.Town == deletTown
                select e).ToList();
            
            foreach (var e in employees)
            {
                e.Address=adress;
            }
            _context.SaveChanges();
            
            foreach (var a in deletAdress)
            {
                var del = _context.Addresses.Remove(a);
                _context.SaveChanges();
            }

            _context.Towns.Remove(deletTown);
            _context.SaveChanges();
            Console.WriteLine($"{townName} удалён.");
        }
    }

}