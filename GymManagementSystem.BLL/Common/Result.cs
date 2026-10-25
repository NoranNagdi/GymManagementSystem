using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.Common
{
    public sealed record Result(bool success, string? error = null, ResultKind kind = ResultKind.OK)
    {
        public static Result Ok() => new(true);
        public static Result NotFound(string error = "Not Found") => new(false, error, ResultKind.NotFound);
        public static Result Validation(string error) => new(false, error, ResultKind.ValidationFalied);
        public static Result Fail(string error) => new(false, error, ResultKind.Conflict);
    }
    public sealed record Result<T>(bool success, T? value, string? error = null, ResultKind kind = ResultKind.OK)
    {
        public static Result<T> Ok(T value) => new Result<T>(true, value);
        public static Result<T> NotFound(string error = "Not Found") => new Result<T>(false, default, error, ResultKind.NotFound);
        public static Result<T> Validation(string error) => new Result<T>(false, default, error, ResultKind.ValidationFalied);
        public static Result<T> Fail(string error) => new Result<T>(false, default, error, ResultKind.Conflict);
    }

    public enum ResultKind
    {
        OK,
        NotFound,
        ValidationFalied,
        Conflict,
        Forbidden
    }
}
