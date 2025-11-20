using System;
using System.Collections.Generic;
using System.Linq;
using GeneticProgramming.Expressions;

namespace GeneticProgramming.Expressions.Symbols
{
    /// <summary>
    /// Curated symbol sets organized by semantic purpose.
    /// These static sets allow users to quickly assemble grammars for specific domains.
    /// </summary>
    public static class SymbolSets
    {
        // ========== CORE MATHEMATICAL SETS ==========
        
        /// <summary>
        /// Pure arithmetic operations (addition, subtraction, multiplication, division, power, root)
        /// Fundamental building blocks for any mathematical expression
        /// </summary>
        public static readonly List<ISymbol<double>> Arithmetic = new List<ISymbol<double>>()
            .Concat(MathematicalSymbols.AllSymbols)
            .ToList();

        /// <summary>
        /// Exponential and logarithmic transformations
        /// Essential for modeling growth, decay, and scale transformations
        /// </summary>
        public static readonly List<ISymbol<double>> Exponential = new List<ISymbol<double>>()
            .Concat(MathematicalLogarithmicSymbols.AllSymbols)
            .ToList();

        /// <summary>
        /// Activation functions and smooth transformations (sigmoid, tanh, softclip)
        /// Used for normalization and bounded outputs
        /// </summary>
        public static readonly List<ISymbol<double>> Activation = new List<ISymbol<double>>()
            .Concat(MathematicalHyperbolicSymbols.AllSymbols)
            .Concat(BoundingSymbols.AllSymbols)
            .ToList();

        // ========== LOGICAL & COMPARISON SETS ==========

        /// <summary>
        /// Comparison and logical operations (>, <, sign, if-else)
        /// Enables conditional logic and decision-making in expressions
        /// </summary>
        public static readonly List<ISymbol<double>> Logic = new List<ISymbol<double>>()
            .Concat(ComparisonSymbols.AllSymbols)
            .Concat(StatisticsSymbols.AllSymbols.Where(s => s.Name == "IfElse"))
            .ToList();

        // ========== AGGREGATION & STATISTICS SETS ==========

        /// <summary>
        /// Simple aggregation operations (mean, max, min) with fixed arity
        /// Zero-allocation statistics for high-performance loops
        /// </summary>
        public static readonly List<ISymbol<double>> Aggregation = new List<ISymbol<double>>()
            .Concat(StatisticsSymbols.AllSymbols.Where(s => 
                s.Name == "Mean2" || s.Name == "Mean3" || s.Name == "Mean4" || s.Name == "Mean5" || s.Name == "Mean6" ||
                s.Name == "Max2" || s.Name == "Max3" || s.Name == "Max4" || s.Name == "Max5" || s.Name == "Max6" ||
                s.Name == "Min2" || s.Name == "Min3" || s.Name == "Min4" || s.Name == "Min5" || s.Name == "Min6"))
            .ToList();

        /// <summary>
        /// Advanced statistical operations (variance, median) - variadic
        /// WARNING: Allocates arrays - use sparingly in performance-critical code
        /// </summary>
        public static readonly List<ISymbol<double>> AdvancedStatistics = new List<ISymbol<double>>()
            .Concat(StatisticsSymbols.AllSymbols.Where(s => 
                s.Name == "Mean" || s.Name == "Variance" || s.Name == "Median"))
            .ToList();

        // ========== FINANCIAL/TRADING-SPECIFIC SETS ==========

        /// <summary>
        /// Normalization and ratio operations (percent change, z-score, min-max normalization)
        /// Core for technical indicators and feature engineering
        /// </summary>
        public static readonly List<ISymbol<double>> Normalization = new List<ISymbol<double>>()
            .Concat(RatioSymbols.AllSymbols)
            .ToList();

        /// <summary>
        /// Time-series analysis operations (momentum, rate of change, decay)
        /// Captures temporal dynamics and trends
        /// </summary>
        public static readonly List<ISymbol<double>> TimeSeries = new List<ISymbol<double>>()
            .Concat(TemporalSymbols.AllSymbols)
            .Concat(MovingAverageSymbols.AllSymbols)
            .ToList();

        /// <summary>
        /// Complete technical analysis toolkit
        /// Combines normalization, time-series, and aggregation for trading strategies
        /// </summary>
        public static readonly List<ISymbol<double>> TechnicalAnalysis = new List<ISymbol<double>>()
            .Concat(Normalization)
            .Concat(TimeSeries)
            .Concat(Aggregation)
            .Concat(Logic)
            .ToList();

        // ========== COMPOSITE SETS BY USE CASE ==========

        /// <summary>
        /// Minimal symbolic regression set
        /// Pure mathematics without domain-specific operations
        /// </summary>
        public static readonly List<ISymbol<double>> SymbolicRegressionMinimal = new List<ISymbol<double>>()
            .Concat(Arithmetic)
            .Concat(Exponential)
            .ToList();

        /// <summary>
        /// Complete symbolic regression set
        /// Adds trigonometry and advanced math for complex fitting
        /// </summary>
        public static readonly List<ISymbol<double>> SymbolicRegressionComplete = new List<ISymbol<double>>()
            .Concat(SymbolicRegressionMinimal)
            .Concat(MathematicalTrigonometricSymbols.AllSymbols)
            .Concat(Aggregation)
            .ToList();

        /// <summary>
        /// Fast-evaluation optimized set
        /// Only low-cost operations (excludes logs, trig, variadic)
        /// Ideal for real-time systems or large populations
        /// </summary>
        public static readonly List<ISymbol<double>> FastEvaluation = new List<ISymbol<double>>()
            .Concat(MathematicalSymbols.AllSymbols)
            .Concat(MathematicalLogarithmicSymbols.AllSymbols.Where(s => s.Name == "Exponential"))
            .Concat(BoundingSymbols.AllSymbols.Where(s => s.Name == "SoftClip"))
            .Concat(ComparisonSymbols.AllSymbols)
            .Concat(StatisticsSymbols.AllSymbols.Where(s => 
                s.Name == "Mean2" || s.Name == "Max2" || s.Name == "Min2"))
            .ToList();

        /// <summary>
        /// Lightweight trading set
        /// Essential operations for price-based strategies (no advanced stats)
        /// Balance between expressiveness and speed
        /// </summary>
        public static readonly List<ISymbol<double>> TradingLightweight = new List<ISymbol<double>>()
            .Concat(Arithmetic)
            .Concat(Exponential)
            .Concat(Activation)
            .Concat(Logic)
            .Concat(Normalization)
            .Concat(Aggregation)
            .ToList();

        /// <summary>
        /// Full trading set
        /// Includes time-series analysis for momentum/trend strategies
        /// Recommended starting point for trading bots
        /// </summary>
        public static readonly List<ISymbol<double>> TradingFull = new List<ISymbol<double>>()
            .Concat(TradingLightweight)
            .Concat(TimeSeries)
            .ToList();

        /// <summary>
        /// Advanced trading set
        /// Adds variadic statistics and trigonometry
        /// WARNING: Higher computational cost - use if simpler sets underfit
        /// </summary>
        public static readonly List<ISymbol<double>> TradingAdvanced = new List<ISymbol<double>>()
            .Concat(TradingFull)
            .Concat(AdvancedStatistics)
            .Concat(MathematicalTrigonometricSymbols.AllSymbols)
            .ToList();
    }
}
