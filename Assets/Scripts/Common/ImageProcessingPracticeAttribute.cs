using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class ImageProcessingPracticeAttribute : Attribute
{
    public int Number { get; }
    public string Description { get; }

    public ImageProcessingPracticeAttribute(int number, string description = null)
    {
        this.Number = number;

        if (string.IsNullOrEmpty(description)) {
            this.Description = $"{this.Number:000}";
        } else {
            this.Description = $"{this.Number:000}: {description}";
        }
    }
}