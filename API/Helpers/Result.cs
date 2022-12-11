﻿namespace API.Helpers;
public class Result<T>
{
    public bool IsSuccess { get; set; }
    public T Value { get; set; }
    public string Error { get; set; }
    public List<string> Errors { get; set; }

    public static Result<T> Success(T value) => new() { IsSuccess = true, Value = value };
    public static Result<T> Failure(List<string> errors) => new() { IsSuccess = false, Errors = errors };
}
