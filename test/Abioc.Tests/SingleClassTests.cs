﻿// Copyright (c) 2017 James Skimming. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace Abioc
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Abioc.Composition;
    using Abioc.Registration;
    using FluentAssertions;
    using Xunit;

    public abstract class WhenCreatingASimpleClassWithoutDependenciesBase
    {
        public abstract TService GetService<TService>();

        public abstract IEnumerable<TService> GetServices<TService>();

        [Fact]
        public void ItShouldCreateTheService()
        {
            // Act
            SimpleClass1WithoutDependencies actual = GetService<SimpleClass1WithoutDependencies>();

            // Assert
            actual.Should().NotBeNull();
        }

        [Fact]
        public void ItShouldCreateServices()
        {
            // Act
            IReadOnlyList<SimpleClass1WithoutDependencies> actual =
                GetServices<SimpleClass1WithoutDependencies>().ToList();

            // Assert
            actual.Should().HaveCount(1);
            actual[0].Should().NotBeNull();
        }

        [Fact]
        public void ItShouldThrowADiExceptionIfGettingAnUnregisteredService()
        {
            // Arrange
            string expectedMessage =
                "There is no registered factory to create services of type" +
                $" '{typeof(SimpleClass2WithoutDependencies)}'.";

            // Act
            Action action = () => GetService<SimpleClass2WithoutDependencies>();

            // Assert
            action
                .ShouldThrow<DiException>()
                .WithMessage(expectedMessage);
        }

        [Fact]
        public void ItShouldReturnAnEmptyListIfGettingUnregisteredServices()
        {
            // Act
            IEnumerable<SimpleClass2WithoutDependencies> actual =
                GetServices<SimpleClass2WithoutDependencies>();

            // Assert
            // Assert
            actual.Should().BeEmpty();
        }
    }

    public class WhenCreatingASimpleClassWithoutDependenciesWithAContext
        : WhenCreatingASimpleClassWithoutDependenciesBase
    {
        private readonly AbiocContainer<int> _container;

        public WhenCreatingASimpleClassWithoutDependenciesWithAContext()
        {
            _container =
                new RegistrationSetup<int>()
                    .Register<SimpleClass1WithoutDependencies>()
                    .Construct(GetType().GetTypeInfo().Assembly);
        }

        public override TService GetService<TService>() => _container.GetService<TService>(1);

        public override IEnumerable<TService> GetServices<TService>() => _container.GetServices<TService>(1);
    }

    public class WhenCreatingASimpleClassWithoutDependenciesWithoutAContext
        : WhenCreatingASimpleClassWithoutDependenciesBase
    {
        private readonly AbiocContainer _container;

        public WhenCreatingASimpleClassWithoutDependenciesWithoutAContext()
        {
            _container =
                new RegistrationSetup()
                    .Register<SimpleClass1WithoutDependencies>()
                    .Construct(GetType().GetTypeInfo().Assembly);
        }

        public override TService GetService<TService>() => _container.GetService<TService>();

        public override IEnumerable<TService> GetServices<TService>() => _container.GetServices<TService>();
    }

    public class WhenFactoringASimpleClassWithoutDependencies
    {
        private readonly SimpleClass1WithoutDependencies _expected;

        private readonly CompilationContext<DefaultContructionContext> _context;

        public WhenFactoringASimpleClassWithoutDependencies()
        {
            // Arrange
            _expected = new SimpleClass1WithoutDependencies();

            _context = new RegistrationContext<DefaultContructionContext>()
                .Register(c => _expected)
                .Compile(GetType().GetTypeInfo().Assembly);
        }

        [Fact]
        public void ItShouldCreateTheServiceUsingTheGivenFactory()
        {
            // Act
            SimpleClass1WithoutDependencies actual = _context.GetService<SimpleClass1WithoutDependencies>();

            // Assert
            actual
                .Should()
                .NotBeNull()
                .And.BeSameAs(_expected);
        }

        [Fact]
        public void ItShouldCreateServicesUsingTheGivenFactory()
        {
            // Act
            IReadOnlyList<SimpleClass1WithoutDependencies> actual =
                _context.GetServices<SimpleClass1WithoutDependencies>().ToList();

            // Assert
            actual.Should().HaveCount(1);
            actual[0].Should().BeSameAs(_expected);
        }

        [Fact]
        public void ItShouldThrowADiExceptionIfGettingAnUnregisteredService()
        {
            // Arrange
            string expectedMessage =
                "There is no registered factory to create services of type" +
                $" '{typeof(SimpleClass2WithoutDependencies)}'.";

            // Act
            Action action = () => _context.GetService<SimpleClass2WithoutDependencies>();

            // Assert
            action
                .ShouldThrow<DiException>()
                .WithMessage(expectedMessage);
        }

        [Fact]
        public void ItShouldReturnAnEmptyListIfGettingUnregisteredServices()
        {
            // Act
            IEnumerable<SimpleClass2WithoutDependencies> actual =
                _context.GetServices<SimpleClass2WithoutDependencies>();

            // Assert
            actual.Should().BeEmpty();
        }
    }
}
