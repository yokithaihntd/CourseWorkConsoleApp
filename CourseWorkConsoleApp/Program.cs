using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace CourseWorkConsoleApp
{
    internal enum PositionType
    {
        Викладач = 1,
        Доцент,
        Професор,
        Методист,
        Електрик,
        Ректор
    }

    internal class Program
    {
        public DataContext _context;
        public List<Employee> EmployeesList = new List<Employee>();

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            Program program = new Program
            {
                _context = new DataContext()
            };
            program._context.Database.EnsureCreated();

            bool exitRequested = false;
            while (!exitRequested)
            {
                Console.WriteLine("1. Додати працівника");
                Console.WriteLine("2. Змінити працівника");
                Console.WriteLine("3. Видалити працівника");
                Console.WriteLine("4. Показати всіх працівників");
                Console.WriteLine("5. Пошук працівника");
                Console.WriteLine("6. Вийти");
                Console.Write("Виберіть опцію: ");
                string option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        program.AddEmployee();
                        Console.WriteLine();
                        break;
                    case "2":
                        program.UpdateEmployee();
                        Console.WriteLine();
                        break;
                    case "3":
                        program.DeleteEmployee();
                        Console.WriteLine();
                        break;
                    case "4":
                        program.DisplayEmployees();
                        Console.WriteLine();
                        break;
                    case "5":
                        program.SearchEmployee();
                        Console.WriteLine();
                        break;
                    case "6":
                        exitRequested = true;
                        break;
                    default:
                        Console.WriteLine("Невірний вибір опції. Спробуйте ще раз.");
                        break;
                }
            }
        }

        // Метод для додавання нового працівника
        private void AddEmployee()
        {
            // Зчитування даних про працівника з консолі
            Console.Write("Введіть прізвище: ");
            string lastName = Console.ReadLine();

            Console.Write("Введіть ім'я: ");
            string firstName = Console.ReadLine();

            Console.Write("Введіть по батькові: ");
            string patronymic = Console.ReadLine();

            // Вибір посади працівника зі списку
            Console.WriteLine("Виберіть посаду\n1. Викладач\n2. Доцент\n3. Професор\n4. Методист\n5. Електрик\n6. Ректор");
            int choice;
            if (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > 6)
            {
                Console.WriteLine("Помилка: Введено неправильне значення. Будь ласка, виберіть від 1 до 6.");
                return;
            }

            string position = GetPosition((PositionType)choice); // Отримання назви посади з enum
            double salary = GetSalary(position); // Отримання зарплати для вибраної посади
            bool salaryPerHour = choice >= 1 && choice <= 3;

            // Створення нового працівника з введеними даними
            Employee employee = CreateEmployee(lastName, firstName, patronymic, position, salary, salaryPerHour);
            if (employee != null)
            {
                SaveEmployee(employee); // Збереження працівника у базі даних
                Console.WriteLine($"Тип зарплати: {(salaryPerHour ? "Погодинна" : "Фіксована")}");
                Console.WriteLine("Співробітник успішно доданий!");
            }
            else
            {
                Console.WriteLine("Не вдалося створити працівника. Перевірте введені дані.");
            }
        }

        // Метод для отримання годинної ставки залежно від посади
        private double GetHourlyRate(string position)
        {
            switch (position)
            {
                case "Викладач":
                    return 100;
                case "Доцент":
                    return 170;
                case "Професор":
                    return 200;
                default:
                    Console.WriteLine("Помилка: Некоректна посада.");
                    return 0;
            }
        }

        // Метод для отримання назви посади з enum
        private string GetPosition(PositionType positionType)
        {
            return positionType.ToString();
        }

        // Метод для отримання зарплати залежно від посади
        private double GetSalary(string position)
        {
            switch (position)
            {
                case "Викладач":
                    return 100;
                case "Доцент":
                    return 170;
                case "Професор":
                    return 200;
                case "Методист":
                case "Електрик":
                case "Ректор":
                    Console.Write($"Введіть зарплату для {position}: ");
                    if (!double.TryParse(Console.ReadLine(), out double customSalary) || customSalary < 0)
                    {
                        Console.WriteLine("Помилка: Введено некоректну зарплату.");
                        return 0;
                    }
                    return customSalary;
                default:
                    return 0;
            }
        }

        // Метод для створення об'єкту працівника з введеними даними
        private Employee CreateEmployee(string lastName, string firstName, string patronymic, string position, double salary, bool salaryPerHour)
        {
            Employee employee;
            if (salaryPerHour)
            {
                HourlyEmployee hourlyEmployee = new HourlyEmployee();
                Console.Write("Введіть кількість відпрацьованих годин: ");
                if (!int.TryParse(Console.ReadLine(), out int workedHours) || workedHours < 0)
                {
                    Console.WriteLine("Помилка: Введено некоректну кількість годин.");
                    return null;
                }
                hourlyEmployee.WorkedHours = workedHours;
                hourlyEmployee.HourlyRate = GetHourlyRate(position);
                hourlyEmployee.Salary = hourlyEmployee.CalculateTotalSalary();

                employee = hourlyEmployee;
            }
            else
            {
                SalariedEmployee salariedEmployee = new SalariedEmployee();
                salariedEmployee.Salary = salary;

                employee = salariedEmployee;
            }

            employee.LastName = lastName;
            employee.FirstName = firstName;
            employee.Patronymic = patronymic;
            employee.Position = position;

            return employee;
        }

        // Метод для збереження працівника в базі даних
        private void SaveEmployee(Employee employee)
        {
            if (employee is HourlyEmployee hourlyEmployee)
            {
                _context.HourlyEmployees.Add(hourlyEmployee);
            }
            else if (employee is SalariedEmployee salariedEmployee)
            {
                _context.SalariedEmployees.Add(salariedEmployee);
            }
            _context.SaveChanges();
        }

        // Метод для оновлення інформації про працівника
        private void UpdateEmployee()
        {
            // Зчитування ID працівника, інформацію про якого потрібно оновити
            Console.Write("Введіть ID працівника, інформацію про якого потрібно оновити: ");
            int id = Convert.ToInt32(Console.ReadLine());
            var employee = _context.HourlyEmployees.FirstOrDefault(e => e.Id == id) ?? (Employee)_context.SalariedEmployees.FirstOrDefault(e => e.Id == id);

            if (employee != null)
            {
                // Вибір, яку інформацію оновити
                Console.WriteLine("Оберіть, яку інформацію потрібно оновити:");
                Console.WriteLine("1. Прізвище");
                Console.WriteLine("2. Ім'я");
                Console.WriteLine("3. По батькові");
                Console.WriteLine("4. Посаду");
                Console.WriteLine("5. Зарплату");
                Console.Write("Виберіть опцію: ");
                string option = Console.ReadLine();

                switch (option)
                {
                    // Оновлення прізвища
                    case "1":
                        Console.Write("Введіть нове прізвище: ");
                        employee.LastName = Console.ReadLine();
                        break;
                    // Оновлення ім'я
                    case "2":
                        Console.Write("Введіть нове ім'я: ");
                        employee.FirstName = Console.ReadLine();
                        break;
                    // Оновлення по батькові
                    case "3":
                        Console.Write("Введіть нове по батькові: ");
                        employee.Patronymic = Console.ReadLine();
                        break;
                    // Оновлення посади
                    case "4":
                        Console.WriteLine("Виберіть нову посаду:");
                        Console.WriteLine("1. Викладач");
                        Console.WriteLine("2. Доцент");
                        Console.WriteLine("3. Професор");
                        Console.WriteLine("4. Методист");
                        Console.WriteLine("5. Електрик");
                        Console.WriteLine("6. Ректор");
                        int choice;
                        if (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > 6)
                        {
                            Console.WriteLine("Помилка: Введено неправильне значення. Будь ласка, виберіть від 1 до 6.");
                            return;
                        }
                        employee.Position = GetPosition((PositionType)choice);
                        break;
                    // Оновлення зарплати
                    case "5":
                        Console.Write("Введіть нову зарплату: ");
                        double newSalary;
                        if (!double.TryParse(Console.ReadLine(), out newSalary) || newSalary < 0)
                        {
                            Console.WriteLine("Помилка: Введено некоректну зарплату.");
                            return;
                        }
                        if (employee is HourlyEmployee)
                        {
                            Console.WriteLine("Працівник працює за годинною оплатою. Змінення зарплати за годину не підтримується.");
                            return;
                        }
                        else
                        {
                            ((SalariedEmployee)employee).Salary = newSalary;
                        }
                        break;
                    default:
                        Console.WriteLine("Невірний вибір опції.");
                        return;
                }

                _context.SaveChanges();
                Console.WriteLine($"Інформація про працівника з ID {id} успішно оновлена.");
            }
            else
            {
                Console.WriteLine($"Працівник з ID {id} не знайдений.");
            }
        }

        // Метод для видалення працівника з бази даних
        private void DeleteEmployee()
        {
            // Зчитування ID працівника, якого потрібно видалити
            Console.Write("Введіть ID працівника, якого потрібно видалити: ");
            int id = Convert.ToInt32(Console.ReadLine());
            var employee = _context.HourlyEmployees.FirstOrDefault(e => e.Id == id) ?? (Employee)_context.SalariedEmployees.FirstOrDefault(e => e.Id == id);
            if (employee != null)
            {
                if (employee is HourlyEmployee hourlyEmployee)
                    _context.HourlyEmployees.Remove(hourlyEmployee);
                else if (employee is SalariedEmployee salariedEmployee)
                    _context.SalariedEmployees.Remove(salariedEmployee);

                _context.SaveChanges();
                Console.WriteLine($"Працівник з ID {id} успішно видалений.");
            }
            else
            {
                Console.WriteLine($"Працівник з ID {id} не знайдений.");
            }
        }

        // Метод для відображення списку працівників
        void DisplayEmployees()
        {
            if (!_context.HourlyEmployees.Any() && !_context.SalariedEmployees.Any())
            {
                Console.WriteLine("Список працівників порожній.");
                return;
            }

            var allHourlyEmployees = _context.HourlyEmployees
                .Select(e => new { Employee = e, Salary = e.WorkedHours * e.HourlyRate });

            var allSalariedEmployees = _context.SalariedEmployees
                .Select(e => new { Employee = e, Salary = e.Salary });

            Console.WriteLine("Оберіть тип сортування:");
            Console.WriteLine("1. Сортування за прізвищем");
            Console.WriteLine("2. Сортування за зарплатою");
            Console.Write("Виберіть опцію: ");
            string sortingOption = Console.ReadLine();

            switch (sortingOption)
            {
                case "1":
                    allHourlyEmployees = allHourlyEmployees.OrderBy(e => e.Employee.LastName);
                    allSalariedEmployees = allSalariedEmployees.OrderBy(e => e.Employee.LastName);
                    break;
                case "2":
                    allHourlyEmployees = allHourlyEmployees.OrderByDescending(e => e.Salary);
                    allSalariedEmployees = allSalariedEmployees.OrderByDescending(e => e.Salary);
                    break;
                default:
                    Console.WriteLine("Невірний вибір опції. Сортування за зарплатою буде застосоване за замовчуванням.");
                    allHourlyEmployees = allHourlyEmployees.OrderByDescending(e => e.Salary);
                    allSalariedEmployees = allSalariedEmployees.OrderByDescending(e => e.Salary);
                    break;
            }

            Console.WriteLine("Часові працівники:");
            foreach (var employee in allHourlyEmployees)
            {
                Console.WriteLine($"Id - {employee.Employee.Id}, Прізвище - {employee.Employee.LastName}, Ім'я - {employee.Employee.FirstName}, По-батькові - {employee.Employee.Patronymic}, Посада - {employee.Employee.Position}, Зарплата - {employee.Salary}");
            }

            Console.WriteLine("Працівники з фіксованою зарплатою:");
            foreach (var employee in allSalariedEmployees)
            {
                Console.WriteLine($"Id - {employee.Employee.Id}, Прізвище - {employee.Employee.LastName}, Ім'я - {employee.Employee.FirstName}, По-батькові - {employee.Employee.Patronymic}, Посада - {employee.Employee.Position}, Зарплата - {employee.Salary}");
            }
        }

        // Метод для пошуку працівника
        void SearchEmployee()
        {
            bool searchCompleted = false;

            while (!searchCompleted)
            {
                Console.WriteLine("Пошук працівника:");
                Console.WriteLine("1. Знайти працівників за ім'ям");
                Console.WriteLine("2. Знайти працівників, які працюють за погодинною оплатою");
                Console.WriteLine("3. Знайти тільки доцентів і обчислити зарплату за місяць кожного");
                Console.WriteLine("4. Відібрати працівників, які працюють на окладі");
                Console.WriteLine("5. Знайти співробітників, у яких зарплата більше вказаного розміру");
                Console.WriteLine("6. Повернутися");
                Console.Write("Виберіть опцію: ");
                string option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        SearchByName();
                        searchCompleted = true;
                        break;
                    case "2":
                        DisplayEmployees(SearchHourlyEmployees());
                        searchCompleted = true;
                        break;
                    case "3":
                        DisplayEmployees(SearchAssociateProfessors());
                        searchCompleted = true;
                        break;
                    case "4":
                        DisplayEmployees(SearchSalariedEmployees());
                        searchCompleted = true;
                        break;
                    case "5":
                        DisplayEmployees(SearchHighPaidEmployees());
                        searchCompleted = true;
                        break;
                    case "6":
                        searchCompleted = true;
                        break;
                    default:
                        Console.WriteLine("Невірний вибір опції.");
                        break;
                }
            }
        }
        // Метод для пошуку працівника за ім'ям
        void SearchByName()
        {
            Console.WriteLine("Пошук працівника за ім'ям:");
            string name = Console.ReadLine();

            // Знайти всіх працівників з годинною оплатою, які мають співпадіння в імені, прізвищі або по-батькові
            var hourlyEmployees = _context.HourlyEmployees
                .Where(e => e.FirstName.Contains(name) || e.LastName.Contains(name) || e.Patronymic.Contains(name))
                .ToList();

            // Знайти всіх працівників з фіксованою зарплатою, які мають співпадіння в імені, прізвищі або по-батькові
            var salariedEmployees = _context.SalariedEmployees
                .Where(e => e.FirstName.Contains(name) || e.LastName.Contains(name) || e.Patronymic.Contains(name))
                .ToList();

            // Об'єднати знайдених працівників обох типів у єдиний список
            var employees = hourlyEmployees.Cast<Employee>().ToList().Union(salariedEmployees.Cast<Employee>()).ToList();

            // Відобразити знайдених працівників
            DisplayEmployees(employees);
        }

        // Метод для пошуку всіх працівників з годинною оплатою
        List<Employee> SearchHourlyEmployees()
        {
            var hourlyEmployees = _context.HourlyEmployees.ToList<Employee>();
            return hourlyEmployees.Cast<Employee>().ToList();
        }

        // Метод для пошуку всіх доцентів
        List<Employee> SearchAssociateProfessors()
        {
            // Знайти всіх працівників з годинною оплатою, які є доцентами
            var associateProfessors = _context.HourlyEmployees
                .Where(e => e.Position == "Доцент")
                .ToList<Employee>();
            return associateProfessors;
        }

        // Метод для пошуку всіх працівників з фіксованою зарплатою
        List<Employee> SearchSalariedEmployees()
        {
            var salariedEmployees = _context.SalariedEmployees.ToList<Employee>();
            return salariedEmployees;
        }

        // Метод для пошуку працівників з високою зарплатою
        List<Employee> SearchHighPaidEmployees()
        {
            Console.Write("Введіть мінімальний розмір зарплати: ");
            if (!double.TryParse(Console.ReadLine(), out double minSalary))
            {
                Console.WriteLine("Помилка: Некоректне значення зарплати.");
                return new List<Employee>();
            }

            // Знайти всіх працівників з фіксованою зарплатою, зарплата яких більша за вказаний розмір
            var highPaidEmployees = _context.SalariedEmployees
                .Where(e => e.Salary > minSalary)
                .ToList<Employee>();
            return highPaidEmployees;
        }

        // Метод для відображення списку працівників
        void DisplayEmployees(List<Employee> employees)
        {
            if (employees.Any())
            {
                Console.WriteLine($"Знайдено працівників: {employees.Count}");
                foreach (var employee in employees)
                {
                    // Перевірити, чи працівник є працівником з годинною оплатою чи з фіксованою зарплатою та відображення відповідно
                    if (employee is HourlyEmployee hourlyEmployee)
                    {
                        string salaryType = "Погодинна";
                        double salary = hourlyEmployee.WorkedHours * hourlyEmployee.HourlyRate;
                        Console.WriteLine($"ID - {employee.Id}, Прізвище - {employee.LastName}, Ім'я - {employee.FirstName}, По-батькові - {employee.Patronymic}, Посада - {employee.Position}, Зарплата - {salary}, Ознака зарплати - {salaryType}");
                    }
                    else if (employee is SalariedEmployee salariedEmployee)
                    {
                        Console.WriteLine($"ID - {employee.Id}, Прізвище - {employee.LastName}, Ім'я - {employee.FirstName}, По-батькові - {employee.Patronymic}, Посада - {employee.Position}, Зарплата - {salariedEmployee.Salary}, Ознака зарплати - Фіксована");
                    }
                }
            }
            else
            {
                Console.WriteLine("Не знайдено працівників, що відповідають запиту.");
            }
        }
    }
}