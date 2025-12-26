using System.ComponentModel.DataAnnotations;

namespace RateMyResto.Features.Event.Attributs;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class DateNotInPastAttribute : ValidationAttribute
{
    public DateNotInPastAttribute()
    {
        ErrorMessage = "La date ne peut pas être antérieure à aujourd'hui.";
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
        {
            // Laisser [Required] gérer la nullité.
            return ValidationResult.Success;
        }

        // Gestion DateOnly et DateOnly?
        if (value is DateOnly dateOnly)
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);

            return dateOnly < today 
                ? new ValidationResult(ErrorMessage) 
                : ValidationResult.Success;
        }

        // Gestion DateTime et DateTime?
        if (value is DateTime dateTime)
        {
            DateTime today = DateTime.Today;
            DateTime datePart = dateTime.Date;

            return datePart < today 
                ? new ValidationResult(ErrorMessage) 
                : ValidationResult.Success;
        }

        // Type non supporté
        return new ValidationResult("Type de date non supporté pour la validation.");
    }
}
