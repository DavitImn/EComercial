using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using EComercial.Core;

namespace EComercial.Validators
{
    internal class CustomerValidator : AbstractValidator<Customer>
    {
        public CustomerValidator() 
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name Is Required")
                .Length(3, 20).WithMessage("Customer Name Should be Great Then 3 And Less Then 20 Symbols");
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email Is Required.")
                .Matches(@"^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@gmail\.com$")
                .WithMessage("Email must be a valid Gmail address.");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password Is Required")
                .Length(3, 20).WithMessage("Password Should Be Great Then 3 And Less Then 20 Symbols")
                 .Matches(@"^[A-Z](?=.*[0-9])(?=.*[!@#$%^&*(),.?\"":{}|<>~`_\-+=/\\[\];']).{2,}$")
                .WithMessage("Password must start with a capital letter, include at least one number, one special character, and be at least 3 characters long.");
        }
    }
}
