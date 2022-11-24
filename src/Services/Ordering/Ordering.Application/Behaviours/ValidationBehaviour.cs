﻿using FluentValidation;
using MediatR;

namespace Ordering.Application.Behaviours
{
    public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : MediatR.IRequest<TResponse>
    {

        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken
        )
        {

            if (_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);
                var validationContext = await Task.WhenAll(
                    _validators.Select(
                        v => v.ValidateAsync(context, cancellationToken)
                    )
                );
                var failures = validationContext.SelectMany(r => r.Errors)
                    .Where(f => f != null)
                    .ToList();

                if (failures.Count != 0)
                    throw new ValidationException(failures);

            }

            return await next();
        }
    }
}