/**
 * @author      - Timeplex
 * 
 * @created     - 09.02.2023
 * 
 * @last_change - 09.02.2023
 */
namespace VoTCore.Algorithms
{
    /// <summary>
    /// Class for displaying and handling a Loading Bar in Console
    /// </summary>
    public class LoadingBar
    {
        /// <summary>
        /// Amount of steps needed to reach the 100%
        /// </summary>
        public readonly int Steps;

        /// <summary>
        /// Current set step
        /// </summary>
        private int currentStep;
        public int CurrentStep { get => currentStep; }

        /// <summary>
        /// Shows if the loading bar is currently activated
        /// </summary>
        private bool IsActive = false;

        /// <summary>
        /// Amount of chars which can be set with the current Console window lenght
        /// </summary>
        private int CurrentLenght = 0;

        /// <summary>
        /// Default construcotr
        /// </summary>
        /// <param name="steps">Amount of steps until 100% is reached</param>
        public LoadingBar(int steps) 
        {
            Steps = steps;
        }

        /// <summary>
        /// Start the Loading bar and displays it for now on
        /// </summary>
        public void Start()
        {
            if (IsActive) return;
            CurrentLenght = Console.WindowWidth;
            IsActive = true;
            Draw();
        }

        /// <summary>
        /// Set a new currentStep value and automaticly update the loading bar
        /// </summary>
        /// <param name="currentStep">New current step</param>
        public void Update(int currentStep)
        {
            if (!IsActive) return;
            this.currentStep = currentStep;
            CurrentLenght = Console.WindowWidth;
            Draw();
        }

        /// <summary>
        /// Pasue the Loadingbar without reseting the loadingbar -> Hide the loading bar. Can be resumed later
        /// </summary>
        public void Pause()
        {
            if (!IsActive) return;
            Clear();
        }

        /// <summary>
        /// Stop the loadingbar, hide it and reset the current Lenght
        /// </summary>
        public void Stop()
        {
            if (!IsActive) return;
            CurrentLenght = 0;
            Clear();
        }

        /// <summary>
        /// Redraw the loadingbar with the current values
        /// </summary>
        private void Draw()
        {
            if (!IsActive) return;
            var currentPercentage = (double) CurrentStep / Steps ;
            var preString = $"\r{currentPercentage * 100:F2}% [";
            var afterString = "]";

            var remainingChars = CurrentLenght - (preString.Length + afterString.Length);

            if (remainingChars < 0) { Clear();  Console.Write("?"); return; }

            var amountToSet = (int)Math.Round(remainingChars * currentPercentage);

            var fullString = preString;

            for(int i = 0; i < remainingChars; i++) 
            { 
                if(i < amountToSet)
                {
                    fullString += "=";
                    continue;
                }
                fullString += " ";
            }

            fullString += afterString;
            Console.Write(fullString);
        }

        /// <summary>
        /// Clear the Loading bar from the sreen
        /// </summary>
        private void Clear()
        {
            if (!IsActive) return;
            Console.Write('\r');

            for (int i = 0; i < Console.WindowWidth; i++)
            {
                Console.Write(' ');
            }

            Console.Write('\r');
            IsActive = false;
        }
    }
}
