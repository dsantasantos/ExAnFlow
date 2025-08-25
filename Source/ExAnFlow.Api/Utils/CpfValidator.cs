using System.Linq;

namespace ExAnFlow.Api.Utils
{
    public static class CpfValidator
    {
        /// <summary>
        /// Validates a Brazilian CPF number.
        /// </summary>
        /// <param name="cpf">CPF string with or without formatting characters.</param>
        /// <returns>True if CPF is valid; otherwise false.</returns>
        public static bool IsValid(string? cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return false;

            cpf = new string(cpf.Where(char.IsDigit).ToArray());

            if (cpf.Length != 11)
                return false;

            if (cpf.Distinct().Count() == 1)
                return false;

            int[] multiplier1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplier2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            var tempCpf = cpf.Substring(0, 9);

            int sum = 0;
            for (int i = 0; i < 9; i++)
                sum += int.Parse(tempCpf[i].ToString()) * multiplier1[i];

            int remainder = sum % 11;
            remainder = remainder < 2 ? 0 : 11 - remainder;
            string digit = remainder.ToString();

            tempCpf += digit;
            sum = 0;
            for (int i = 0; i < 10; i++)
                sum += int.Parse(tempCpf[i].ToString()) * multiplier2[i];

            remainder = sum % 11;
            remainder = remainder < 2 ? 0 : 11 - remainder;
            digit += remainder.ToString();

            return cpf.EndsWith(digit);
        }
    }
}
