﻿// Copyright (c) 2017 James Skimming. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace Abioc.Composition.Compositions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A base class that provides a default implementation of a <see cref="IComposition"/>.
    /// </summary>
    public abstract class CompositionBase : IComposition
    {
        /// <inheritdoc />
        public abstract Type Type { get; }

        /// <inheritdoc />
        public abstract string GetInstanceExpression(CompositionContext context, bool simpleName);

        /// <inheritdoc />
        public abstract string GetComposeMethodName(CompositionContext context, bool simpleName);

        /// <inheritdoc />
        public virtual IEnumerable<string> GetMethods(CompositionContext context, bool simpleName)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            return Enumerable.Empty<string>();
        }

        /// <inheritdoc />
        public virtual IEnumerable<(string snippet, object value)> GetFieldInitializations(CompositionContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            return Enumerable.Empty<(string, object)>();
        }

        /// <inheritdoc />
        public virtual IEnumerable<string> GetFields(CompositionContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            return Enumerable.Empty<string>();
        }

        /// <inheritdoc />
        public abstract bool RequiresConstructionContext(CompositionContext context);

        /// <summary>
        /// Gets all the compositions for the specified <paramref name="types"/>.
        /// </summary>
        /// <param name="context">The <see cref="CompositionContext"/>.</param>
        /// <param name="types">The types for which to retrieve the compositions.</param>
        /// <returns>All the compositions for the specified <paramref name="types"/>.</returns>
        protected static IEnumerable<IComposition> GetCompositions(CompositionContext context, IEnumerable<Type> types)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (types == null)
                throw new ArgumentNullException(nameof(types));

            foreach (Type type in types)
            {
                if (context.Compositions.TryGetValue(type, out IComposition composition))
                {
                    yield return composition;
                }
                else
                {
                    throw new CompositionException($"There is no composition for the type: '{type}'.");
                }
            }
        }
    }
}