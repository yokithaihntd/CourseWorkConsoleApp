using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CourseWorkConsoleApp
{
    // Абстрактний клас, що представляє загальну інформацію про працівника
    abstract class Employee
    {
        [Key]
        public int Id { get; set; } // Унікальний ідентифікатор працівника
        public string LastName { get; set; } // Прізвище працівника
        public string FirstName { get; set; } // Ім'я працівника
        public string Patronymic { get; set; } // По-батькові працівника
        public string Position { get; set; } // Посада працівника

        // Перевизначений метод для отримання рядкового представлення працівника
        public override string ToString()
            => $"Id - {Id}, Прізвище - {LastName}, Ім'я - {FirstName}, По-батькові - {Patronymic}, Посада - {Position}";

        public virtual double CalculateTotalSalary()
        {
            return 0;
        }
    }

    // Клас, що відображає працівників з погодинною оплатою
    class HourlyEmployee : Employee
    {
        public int WorkedHours { get; set; } // Кількість відпрацьованих годин
        public double HourlyRate { get; set; } // Погодинна ставка
        /*public bool SalaryPerHour { get; set; } // Ознака оплати за годину*/
        public double Salary { get; set; }

        // Метод для обчислення середньомісячної заробітної плати
        public override double CalculateTotalSalary()
        {
            return WorkedHours * HourlyRate;
        }

        // Перевизначений метод для отримання рядкового представлення працівника з додатковою інформацією
        public override string ToString()
        {
            return base.ToString() + $", WorkedHours: {WorkedHours}";
        }
    }

    // Клас, що відображає працівників з фіксованою заробітною платою
    class SalariedEmployee : Employee 
    {
        public double Salary { get; set; }
        public override double CalculateTotalSalary()
        {
            return Salary;
        }
    }
}
