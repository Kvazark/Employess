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
           //Console.WriteLine(Task2());
          // Console.WriteLine(Task3());
          // Console.WriteLine(Task4());
          // Console.WriteLine(Task5());
          // Console.WriteLine(Task6());
          // Task7();
           Task8();
        }

        static string GetEmployeesInformation()
        {
            var employees = _context.Employees
                .OrderBy(e => e.EmployeeId)
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.MiddleName,
                    e.JobTitle
                })
                .ToList();
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
            var employees = _context.Employees
                .OrderBy(e => e.Salary)
                .Where(e=> e.Salary > 48000)
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.MiddleName,
                    e.JobTitle,
                    e.DepartmentId,
                    e.ManagerId,
                    e.HireDate,
                    e.Salary,
                    e.AddressId
                })
                .ToList();
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

            var employees = _context.Employees
                .Where(e => e.LastName == "Brown")
                .ToList();
            employees.ForEach(e=>e.Address=adressBrown);
            _context.SaveChanges();
            var sb = new StringBuilder();
            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} {e.MiddleName} {e.JobTitle} {e.DepartmentId} {e.ManagerId} {e.HireDate} {e.Salary} {e.AddressId}");
            }
            return sb.ToString().TrimEnd();
        }

        //Аудит проектов
        static string Task3()
        {
            var specificProjects = _context.EmployeesProjects.Join(_context.Employees,
                    (ep => ep.EmployeeId),
                    (e => e.EmployeeId),
                    (ep, e) => new
                    {
                        FirstName = e.FirstName,
                        LastName = e.LastName,
                        Manager = e.Manager,
                        ProjectName = ep.Project.Name,
                        StartDate = ep.Project.StartDate,
                        EndDate = ep.Project.EndDate
                    })
                .Where(ep => ep.StartDate.Year >= 2002 && ep.StartDate.Year <= 2005)
                .Take(5)
                .ToList();
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
            var employee = _context.Employees.Join(_context.EmployeesProjects,
                    (e => e.EmployeeId),
                    (ep => ep.EmployeeId),
                    (e, ep) => new
                    {
                        EmployeeId = e.EmployeeId,
                        FirstName = e.FirstName, 
                        LastName = e.LastName, 
                        MiddleName = e.MiddleName,
                        JobTitle = e.JobTitle, 
                        ProjectName = ep.Project.Name
                    })
                .Where(e => e.EmployeeId == id)
                .ToList();
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
            var departaments = _context.Departments
                .Where(d => d.Employees.Count()<5)
                .Select(d=> new {d.Name})
                .ToList();
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
            var departament = _context.Departments
                .Select(d => new
                { 
                    d.Name,
                    d.Employees
                })
                .Where(d => d.Name == name)
                .FirstOrDefault();
            Console.Write("Введите на сколько процентов нужно повысить зарплату: ");
            int percent = int.Parse(Console.ReadLine());
            var employees = _context.Employees
                .Where(e => e.Department.Name == name)
                .ToList();
            
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
            var employees = _context.Employees.Where(e => e.Department.DepartmentId == id).Select(e => e).ToList();
            var depatment = _context.Departments.FirstOrDefault(e => e.DepartmentId != id);
            foreach (var e in employees)
            {
                e.DepartmentId=depatment.DepartmentId;
                _context.SaveChanges();
            }

            var deletDepatment = _context.Departments.Where(e => e.DepartmentId == id).ToList();
            foreach (var d in deletDepatment)
            {
                var del = _context.Departments.Remove(d);
                _context.SaveChanges();
            }
        }// попробовать удалить сотрудников вместе с отделом. Пока что они в другой отдел переходят
        
        //Удаление города с заданным названием
        static void Task8()
        {
            Console.Write("Введите название города для его удаления: ");
            string townName = Console.ReadLine();
            var deletTown = _context.Towns.First(t => t.Name == townName);
            
            Address adress = new Address()
            {
                AddressText = "is unknown"
            };
            var deletAdress = _context.Addresses.Where(a => a.Town == deletTown).ToList();
            var employees = _context.Employees
                .Where(e => e.Address.Town == deletTown)
                .ToList();
            
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