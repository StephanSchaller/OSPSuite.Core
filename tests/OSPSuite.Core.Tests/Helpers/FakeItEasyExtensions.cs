﻿using System.Threading.Tasks;
using FakeItEasy;
using FakeItEasy.Configuration;

namespace OSPSuite.Helpers
{
   public static class FakeItEasyExtensions
   {
      public static IAfterCallConfiguredWithOutAndRefParametersConfiguration<IReturnValueConfiguration<Task<TResult>>> ReturnsAsync<TResult>(this IReturnValueArgumentValidationConfiguration<Task<TResult>> valueConfiguration, TResult value)
      {
         return valueConfiguration.Returns(Task.FromResult(value));
      }
   }
}
