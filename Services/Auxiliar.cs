using System.Data;
using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;


namespace API.Services
{
    public static class Auxiliar
    {
        public static Dictionary<string, object>? FirstRowToDictionary(DataTable table)
        {
            return table.Rows.Count > 0
                ? table.Columns.Cast<DataColumn>().ToDictionary(col => col.ColumnName, col => table.Rows[0][col])
                : null;
        }

        public static List<Dictionary<string, object>> DataTableToList(DataTable table)
        {
            return table.AsEnumerable()
                .Select(row => table.Columns.Cast<DataColumn>()
                    .ToDictionary(col => col.ColumnName, col => row[col]))
                .ToList();
        }

    }
    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly int _maxFileSize;

        public MaxFileSizeAttribute(int maxFileSize)
        {
            _maxFileSize = maxFileSize;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var file = value as IFormFile;

            if (file != null && file.Length > _maxFileSize)
            {
                return new ValidationResult($"Max file size is {_maxFileSize / (1024 * 1024)} MB.");
            }

            return ValidationResult.Success;
        }
    }

    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _extensions;

        public AllowedExtensionsAttribute(string[] extensions)
        {
            _extensions = extensions;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var file = value as IFormFile;

            if (file != null)
            {
                var extension = Path.GetExtension(file.FileName).ToLower();
                if (!_extensions.Contains(extension))
                {
                    return new ValidationResult($"Invalid file extension. Allowed: {string.Join(", ", _extensions)}");
                }
            }

            return ValidationResult.Success;
        }
    }
}
