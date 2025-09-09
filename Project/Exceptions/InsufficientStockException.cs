﻿using System;

namespace Project.Exceptions 
{ 
    public class InsufficientStockException : Exception
    {
        public InsufficientStockException(string message) : base(message)
        {
        }
    }
}