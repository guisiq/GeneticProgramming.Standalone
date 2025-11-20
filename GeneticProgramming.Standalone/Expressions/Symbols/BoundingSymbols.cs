using System;
using System.Collections.Generic;
using GeneticProgramming.Expressions;

namespace GeneticProgramming.Expressions.Symbols
{
    /// <summary>
    /// Collection of bounding and clipping functional symbols.
    /// Essential for trading to control ranges and prevent numerical explosions.
    /// </summary>
    public static class BoundingSymbols
    {
        /// <summary>
        /// Clips a value between min and max
        /// </summary>
        public static readonly FunctionalSymbol<double> Clip =
            SymbolFactory<double>.CreateVariadic(
                "Clip", "Clip value between min and max: Clip(value, min, max)",
                args => Math.Max(args[1], Math.Min(args[0], args[2])),
                3, 3);

        /// <summary>
        /// Soft clipping via tanh - maintains differentiability
        /// Output range: (-1, 1)
        /// </summary>
        public static readonly FunctionalSymbol<double> SoftClip =
            SymbolFactory<double>.CreateUnary(
                "SoftClip", "Soft clipping via tanh",
                x => Math.Tanh(x));

        public static readonly List<ISymbol<double>> AllSymbols = new List<ISymbol<double>>
        {
            Clip,
            SoftClip
        };
    }
}
