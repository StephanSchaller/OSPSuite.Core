﻿using System;
using System.Reflection;
using OSPSuite.Assets;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Exceptions;
using OSPSuite.Utility.Extensions;

namespace OSPSuite.Core.Extensions
{
   public static class ExceptionExtensions
   {
      public static string ExceptionMessage(this Exception ex)
      {
         if (IsWrapperException(ex))
            return ExceptionMessage(ex.InnerException);

         return $"{ex.FullMessage()}{Environment.NewLine}{Environment.NewLine}{Captions.ContactSupport(Constants.FORUM_SITE)}";
      }

      public static bool IsWrapperException(this Exception ex)
      {
         return ex.IsAnImplementationOf<TargetInvocationException>() || ex.IsAnImplementationOf<AggregateException>();
      }

      public static string ExceptionMessageWithStackTrace(this Exception ex)
      {
         return $"{ExceptionMessage(ex)}{Environment.NewLine}{Environment.NewLine}Stack trace:{Environment.NewLine}{ex.FullStackTrace()}";
      }

      public static bool IsInfoException(this Exception ex)
      {
         if (ex == null)
            return false;

         if (ex.IsWrapperException())
            return IsInfoException(ex.InnerException);

         if (ex.IsAnImplementationOf<NotFoundException>())
            return false;

         return ex.IsAnImplementationOf<OSPSuiteException>();
      }
   }
}