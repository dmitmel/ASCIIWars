using System;
using System.Threading;
#if !DEBUG
using System.IO;
using System.Linq;
using ASCIIWars.Game;
#endif

namespace ASCIIWars.ConsoleGraphics {
    public class Task {
        public readonly string name;
        public readonly Action action;

        public Task(string name, Action action) {
            this.name = name;
            this.action = action;
        }

        public static Task EmptyTask(string name, int delay) {
            return new Task(name, () => Thread.Sleep(delay));
        }
    }

#if !DEBUG
    /**
     * <summary>
     * Загружает задачи из файла со смешными названиями для задач. 
     * В файле содержатся только имена, а класс преобразует их
     * задачи со случайным временем от <see cref="P:MIN_TASK_DELAY"/>
     * до <see cref="P:MAX_TASK_DELAY"/>.
     * </summary>
     * 
     * <seealso cref="T:Task"/>
     * <seealso cref="M:LoadingBar.Load(List{Task})"/>
     */
    public static class FunnyTasksLoader {
        /// Минимальное время, которое может потробоваться на выполнение загруженой задачи.
        const int MIN_TASK_DELAY = 100;
        /// Максимальное время, которое может потробоваться на выполнение загруженой задачи.
        const int MAX_TASK_DELAY = 500;

        public static Task[] Load() {
            return File.ReadAllLines("assets/funny-task-names.txt").Select(taskName => {
                int taskDelay = GlobalRandom.Next(MIN_TASK_DELAY, MAX_TASK_DELAY);
                return Task.EmptyTask(taskName, taskDelay);
            }).ToArray();
        }
    }
#endif

    /**
     * <summary>
     * Печатает символьную полоску загрузки.
     * </summary>
     * 
     * <remarks>
     * <para>Загрузка выглядит примерно так:
     * <code>
     *  Тут столько же решёток,
     *  На сколько процентов                 Тут столько же пробелов, сколько осталось до
     *  выполнены задачи                     завершения выполнения всех задач
     *           |                                                |
     *  _________|_________  _____________________________________|_______________________________________
     * /                   \/                                                                             \
     * #####################                                                                                21%
     * Hello, World!                                                                                        \_/
     * \___________/                                                                                         |
     *       |                                                                                      Процент выполнения
     * Имя текущей задачи                                                                           всех задач
     * </code></para>
     * 
     * <para>Задач не должно быть больше чем 100, если их меньше,
     * то полоска будет завершена до 100.</para>
     * 
     * <para>Когда выполнение закончится, то имя последней задачи
     * будет заменено на значание <see cref="P:FINISHED_TITLE"/></para>
     * 
     * <para>Консоль очищается, после того, как пройдёт время из
     * <see cref="P:DELAY_BEFORE_CLEARING"/></para>
     * </remarks>
     */
    public static class LoadingBar {
        const string FINISHED_TITLE = "Загрузка завершена!";
#if !DEBUG
        const int DELAY_BEFORE_CLEARING = 1500;
#endif

        public static void Load(Task[] tasks) {
            if (tasks.Length > 100)
                throw new ArgumentException("Слишком много задач");

            int completePercent = 0;
            // Добавляем новою строку, для того, чтобы функция PrintProgressOf
            // смогла "отступить" на 1 линию вверх при 0%
            Console.WriteLine();

            foreach (Task task in tasks) {
                PrintProgressOf(task.name, completePercent);
                task.action.Invoke();
                completePercent++;
            }

            PrintProgressOf(FINISHED_TITLE, 100);

#if !DEBUG
            Thread.Sleep(DELAY_BEFORE_CLEARING);
#endif
            // Полоска загрузки как раз занимает 2 линии
            MyConsole.ClearLine();  // Очищаем линию с текстом о завершении
            MyConsole.MoveCursorUp(1);
            MyConsole.ClearLine();  // Очищаем линию с полоской
        }

        static void PrintProgressOf(string taskName, int completePercent) {
            MyConsole.MoveCursorUp(1);
            MyConsole.MoveCursorToBeginingOfLine();
            Console.Write(new string('#', completePercent));
            Console.Write(new string(' ', 100 - completePercent));
            Console.WriteLine($" {completePercent}%");
            MyConsole.ClearLine();
            Console.Write(taskName);
        }
    }
}

