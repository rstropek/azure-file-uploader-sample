using System.ComponentModel.DataAnnotations;

namespace HelloAspNet
{
    public class MinLengthIfNotNull : ValidationAttribute
    {
        private readonly int minLength;

        public MinLengthIfNotNull(int minLength)
        {
            this.minLength = minLength;
        }

        public override bool IsValid(object? objValue)
        {
            if (objValue == null) return true;

            var stringValue = objValue.ToString();
            return stringValue != null && stringValue.Length >= minLength;
        }
    }
}
