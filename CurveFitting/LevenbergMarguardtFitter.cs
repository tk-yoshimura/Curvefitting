﻿using Algebra;
using DoubleDouble;
using System.Collections.Generic;

namespace CurveFitting {
    /// <summary>Levenberg-MarguardtMethod法</summary>
    public class LevenbergMarquardtFitter : Fitter {
        readonly FittingFunction func;

        /// <summary>コンストラクタ</summary>
        public LevenbergMarquardtFitter(IReadOnlyList<ddouble> xs, IReadOnlyList<ddouble> ys, FittingFunction func)
            : base(xs, ys, func.Parameters) {

            this.func = func;
        }

        /// <summary>フィッティング値</summary>
        public override ddouble FittingValue(ddouble x, Vector parameters) {
            return func.F(x, parameters);
        }

        /// <summary>フィッティング</summary>
        public Vector ExecuteFitting(Vector parameters, double lambda_init = 1, double lambda_decay = 0.9, int iter = 64) {
            Vector errors, dparam;
            Matrix jacobian, jacobian_transpose;

            double lambda = lambda_init;

            for (int j = 0; j < iter; j++) {
                errors = Error(parameters);
                jacobian = Jacobian(parameters);
                jacobian_transpose = jacobian.Transpose;

                dparam = (jacobian_transpose * jacobian + lambda * Matrix.Identity(Parameters)).Inverse * jacobian_transpose * errors;

                if (!Vector.IsValid(dparam)) {
                    break;
                }

                parameters -= dparam;

                lambda *= lambda_decay;
            }

            return parameters;
        }

        /// <summary>ヤコビアン行列</summary>
        private Matrix Jacobian(Vector parameters) {
            Matrix jacobian = new(Points, func.Parameters);

            for (int i = 0, j; i < Points; i++) {
                Vector df = func.DiffF(X[i], parameters);

                for (j = 0; j < parameters.Dim; j++) {
                    jacobian[i, j] = df[j];
                }
            }

            return jacobian;
        }
    }
}