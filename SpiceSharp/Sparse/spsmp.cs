﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiceSharp.Sparse
{
    public static class spsmp
    {
        /// <summary>
        /// SMPaddElt
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="Row"></param>
        /// <param name="Col"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static int SMPaddElt(Matrix matrix, int Row, int Col, double Value)
        {
            spbuild.spGetElement(matrix, Row, Col).Value.Real = Value;
            return matrix.Error;
        }

        /// <summary>
        /// SMPmakeElt
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="Row"></param>
        /// <param name="Col"></param>
        /// <returns></returns>
        public static MatrixElement SMPmakeElt(this Matrix matrix, int Row, int Col)
        {
            return spbuild.spGetElement(matrix, Row, Col);
        }

        /// <summary>
        /// SMPcClear
        /// </summary>
        /// <param name="matrix"></param>
        public static void SMPcClear(Matrix matrix)
        {
            spbuild.spClear(matrix);
        }

        /// <summary>
        /// SMPcLUfac
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="PivTol"></param>
        /// <returns></returns>
        public static int SMPcLUfac(Matrix matrix, double PivTol)
        {
            matrix.spSetComplex();
            return spfactor.spFactor(matrix);
        }

        /// <summary>
        /// SMPluFac
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="PivTol"></param>
        /// <param name="Gmin"></param>
        /// <returns></returns>
        public static int SMPluFac(Matrix matrix, double PivTol, double Gmin)
        {
            matrix.spSetReal();
            LoadGmin(matrix, Gmin);
            return spfactor.spFactor(matrix);
        }

        /// <summary>
        /// SMPcReorder
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="PivTol"></param>
        /// <param name="PivRel"></param>
        /// <param name="NumSwaps"></param>
        /// <returns></returns>
        public static int SMPcReorder(Matrix matrix, double PivTol, double PivRel, out int NumSwaps)
        {
            NumSwaps = 1;
            matrix.spSetComplex();
            return spfactor.spOrderAndFactor(matrix, null, PivRel, PivTol, true);
        }

        /// <summary>
        /// SMPreorder
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="PivTol"></param>
        /// <param name="PivRel"></param>
        /// <param name="Gmin"></param>
        /// <returns></returns>
        public static int SMPreorder(Matrix matrix, double PivTol, double PivRel, double Gmin)
        {
            matrix.spSetReal();
            LoadGmin(matrix, Gmin);
            return spfactor.spOrderAndFactor(matrix, null, PivRel, PivTol, true);
        }

        /// <summary>
        /// SMPcaSolve
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="RHS"></param>
        /// <param name="iRHS"></param>
        /// <param name="Spare"></param>
        /// <param name="iSpare"></param>
        public static void SMPcaSolve(Matrix matrix, double[] RHS, double[] iRHS, double[] Spare, double[] iSpare)
        {
            spsolve.spSolveTransposed(matrix, RHS, RHS, iRHS, iRHS);
        }

        /// <summary>
        /// SMPcSolve
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="RHS"></param>
        /// <param name="iRHS"></param>
        /// <param name="Spare"></param>
        /// <param name="iSpare"></param>
        public static void SMPcSolve(Matrix matrix, double[] RHS, double[] iRHS, double[] Spare, double[] iSpare)
        {
            spsolve.spSolve(matrix, RHS, RHS, iRHS, iRHS);
        }

        /// <summary>
        /// SMPsolve
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="RHS"></param>
        /// <param name="Spare"></param>
        public static void SMPsolve(Matrix matrix, double[] RHS, double[] Spare)
        {
            spsolve.spSolve(matrix, RHS, RHS, null, null);
        }

        /// <summary>
        /// SMPmatSize
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static int SMPmatSize(Matrix matrix)
        {
            return matrix.spGetSize(true);
        }

        /// <summary>
        /// SMPnewMatrix
        /// </summary>
        /// <returns></returns>
        public static Matrix SMPnewMatrix()
        {
            return new Matrix(0, true);
        }

        /*
        /// <summary>
        /// SMPdestroy
        /// </summary>
        /// <param name="matrix"></param>
        public static void SMPdestroy(Matrix matrix)
        {
            matrix.spDestroy();
        }
        */

        /// <summary>
        /// SMPpreOrder
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static int SMPpreOrder(Matrix matrix)
        {
            sputils.spMNA_Preorder(matrix);
            return matrix.Error;
        }

        /// <summary>
        /// SMPprint
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static string SMPprint(Matrix matrix)
        {
            return spoutput.spPrint(matrix, false, true, true);
        }

        /*
         * SMPgetError()
         */
        public static void SMPgetError(Matrix matrix, out int Col, out int Row)
        {
            matrix.spWhereSingular(out Row, out Col);
        }

        public const double M_LN2 = 0.69314718055994530942;
        public const double M_LN10 = 2.30258509299404568402;

        /// <summary>
        /// SMPcDProd
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="pMantissa"></param>
        /// <param name="pExponent"></param>
        /// <returns></returns>
        public static int SMPcDProd(Matrix matrix, MatrixElement pMantissa, out int pExponent)
        {
            double re, im, x, y, z;
            int p;

            sputils.spDeterminant(matrix, out p, out re, out im);

            /* Convert base 10 numbers to base 2 numbers, for comparison */
            y = p * M_LN10 / M_LN2;
            x = (int)y;
            y -= x;

            /* ASSERT
             *	x = integral part of exponent, y = fraction part of exponent
             */

            /* Fold in the fractional part */
            z = Math.Pow(2.0, y);
            re *= z;
            im *= z;

            /* Re-normalize (re or im may be > 2.0 or both < 1.0 */
            if (re != 0.0) {
                y = Math.Log(re, 2.0);
                if (im != 0.0)
                    z = Math.Log(im, 2.0);
                else
                    z = 0;
            } else if (im != 0.0) {
                z = Math.Log(im, 2.0);
                y = 0;
            } else {
                /* Singular */
                /*printf("10 . singular\n");*/
                y = 0;
                z = 0;
            }

            if (y < z)

                y = z;

            pExponent = (int)(x + y);
            x = re * Math.Pow(2.0, (int)-y); // scalb(re, (int)-y);
            z = im * Math.Pow(2.0, (int)-y); // scalb(im, (int)-y);

            pMantissa.Value.Real = x; // scalb(re, (int)-y);
            pMantissa.Value.Imag = y; // scalb(im, (int)-y);

            return matrix.Error;
        }

        //  The following routines need internal knowledge of the Sparse data
        //  structures.

        /// <summary>
        /// Load GMIN
        /// Adds diagonal element contributions for increased convergence.
        /// Not recomended
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="Gmin"></param>
        public static void LoadGmin(Matrix matrix, double Gmin)
        {
            MatrixElement[] Diag = matrix.Diag;

            if (Gmin != 0.0)
            {
                for (int i = 1; i < matrix.Size; i++)
                {
                    if (Diag[i] != null)
                        Diag[i].Value.Real += Gmin;
                }
            }
            return;
        }

        /// <summary>
        /// Find element
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="Row"></param>
        /// <param name="Col"></param>
        /// <param name="CreateIfMissing"></param>
        /// <returns></returns>
        public static MatrixElement SMPfindElt(Matrix matrix, int Row, int Col, bool CreateIfMissing)
        {
            MatrixElement Element;

            Row = matrix.ExtToIntRowMap[Row];
            Col = matrix.ExtToIntColMap[Col];
            Element = spbuild.spcFindElementInCol(matrix, Row, Col, CreateIfMissing);
            return Element;
        }

        // XXX The following should probably be implemented in spUtils

        /*
         * SMPcZeroCol()
         */
        public static int SMPcZeroCol(Matrix matrix, int Col)
        {
            MatrixElement Element;

            Col = matrix.ExtToIntColMap[Col];

            for (Element = matrix.FirstInCol[Col]; Element != null; Element = Element.NextInCol)
            {
                Element.Value.Real = 0.0;
                Element.Value.Imag = 0.0;
            }

            return matrix.Error;
        }

        /// <summary>
        /// SMPcAddCol
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="Accum_Col"></param>
        /// <param name="Addend_Col"></param>
        /// <returns></returns>
        public static int SMPcAddCol(Matrix matrix, int Accum_Col, int Addend_Col)
        {
            MatrixElement Accum, Addend, Prev;

            Accum_Col = matrix.ExtToIntColMap[Accum_Col];
            Addend_Col = matrix.ExtToIntColMap[Addend_Col];

            Addend = matrix.FirstInCol[Addend_Col];
            Prev = null;
            Accum = matrix.FirstInCol[Accum_Col];

            while (Addend != null) {
                while (Accum != null && Accum.Row < Addend.Row)
                {
                    Prev = Accum;
                    Accum = Accum.NextInCol;
                }
                if (Accum == null || Accum.Row > Addend.Row)
                {
                    Accum = spbuild.spcCreateElement(matrix, Addend.Row, Accum_Col, Prev, false);
                }
                Accum.Value.Cplx += Addend.Value.Cplx;
                Addend = Addend.NextInCol;
            }

            return matrix.Error;
        }

        /// <summary>
        /// SMPzeroRow
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="Row"></param>
        /// <returns></returns>
        public static int SMPzeroRow(Matrix matrix, int Row)
        {
            MatrixElement Element;

            Row = matrix.ExtToIntColMap[Row];

            if (matrix.RowsLinked == false)
                spbuild.spcLinkRows(matrix);

            if (matrix.PreviousMatrixWasComplex || matrix.Complex)
            {
                for (Element = matrix.FirstInRow[Row];
                    Element != null;
                    Element = Element.NextInRow)
                {
                    Element.Value.Cplx = 0.0;
                }
            }
            else
            {
                for (Element = matrix.FirstInRow[Row];
                    Element != null;
                    Element = Element.NextInRow)
                {
                    Element.Value.Real = 0.0;
                }
            }

            return matrix.Error;
        }
    }
}