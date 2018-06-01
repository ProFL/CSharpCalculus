// Written by Pedro F Linhares, 2018.
// Computer Engineering undergraduate at the University of Fortaleza.
// Contact: pedroflinhares@edu.unifor.br
//
// This code is released under the LGPLv3

using System;
using System.Threading;

namespace Calculus
{
    internal class ThreadWithResult<T>
    {
        public Thread Thread { get; set; }
        public T Result { get; set; }
        public ThreadState ThreadState { get { return Thread.ThreadState; } }

        public ThreadWithResult(Thread thread)
        {
            Thread = thread;
        }

        public void Start()
        {
            Thread.Start();
        }

        public void Join()
        {
            Thread.Join();
        }
    }

    public class Integration
    {
        /// <summary>
        /// Defines the step to take between x's, the default value is 5e-8 which
        /// is quickly computed by most machines.
        /// </summary>
        protected static double InStep = 0.00000005;

        /// <summary>
        /// Defines the step to take between x's, the default value is 5e-8 which
        /// is quickly computed by most machines.
        /// Value must be between 0 and 1 throws ArgumentOutOfRangeException otherwise.
        /// </summary>
        public static double Step
        {
            get { return InStep; }
            set
            {
                if (value > 0 && value < 1)
                {
                    InStep = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        ///
        /// <summary>
        /// Computes the integration of func from the lesserBound to the upperBound.
        /// In LaTeX notation, it computes this:
        /// $\int_{lesserBound}^{upperBound}{(func(x))(dx)}$
        /// The X variation will be given the the class' Step property.
        /// </summary>
        ///
        /// <param name="func">The function in which to replace x's value</param>
        /// <param name="lowerLimit">The lower integration limit</param>
        /// <param name="upperLimit">The upper intregration limit</param>
        ///
        /// <returns>
        /// A double precision floating point with the integration value.
        /// </returns>
        ///
        public static double Integrate(Func<double, double> func, double lowerLimit, double upperLimit)
        {
            double curStep = lowerLimit;

            double res = 0;
            while (curStep <= upperLimit)
            {
                res += func(curStep) * Step;
                curStep += Step;
            }

            return res;
        }

        ///
        /// <summary>
        /// Computes in parallel the integration of func from the lesserBound to the upperBound.
        /// The parallelism is achieved through dividing the calculation in multiple threads which
        /// will call Integrate(func, lesserBound, lesserBound + 0.5),
        /// Integrate(func, lesserBound + 0.5, lesserBound + 1)... up to upperBound.
        /// Their results then get summed up in the main thread.
        ///
        /// In LaTeX notation, the final result will be given by:
        /// $\int_{lesserBound}^{upperBound}{(func(x))(dx)}$
        ///
        /// The X variation will be given the the class' Step property.
        /// </summary>
        ///
        /// <param name="func">The function in which to replace x's value</param>
        /// <param name="lowerLimit">The lower integration limit</param>
        /// <param name="upperLimit">The upper intregration limit</param>
        ///
        /// <returns>
        /// A double precision floating point with the integration value.
        /// </returns>
        ///
        public static double ParallelIntegration(Func<double, double> func, int lowerLimit, int upperLimit, short maxThreads = 16)
        {
            int neededThrdCnt = (upperLimit - lowerLimit) * 2;
            ThreadWithResult<double>[] threads = new ThreadWithResult<double>[(maxThreads >= neededThrdCnt) ? (upperLimit - lowerLimit) * 2 : maxThreads];
            double result = 0;

            for (int i = 0; i < neededThrdCnt; i++)
            {
                int curThread = i;
                if (curThread >= maxThreads)
                {
                    bool thrdAvail = false;
                    while (!thrdAvail)
                    {
                        for (int thrdStIt = 0; thrdStIt < threads.Length; thrdStIt++)
                        {
                            if (threads[thrdStIt].ThreadState == ThreadState.Stopped)
                            {
                                result += threads[thrdStIt].Result;
                                curThread = thrdStIt;
                                thrdAvail = true;
                                break;
                            }
                        }
                    }
                }

                double curLesserBound = lowerLimit + 0.5 * i;
                double curUpperBound = curLesserBound + 0.5;

                threads[curThread] = new ThreadWithResult<double>(new Thread(() =>
                {
                    threads[curThread].Result = Integrate(func, curLesserBound, curUpperBound);
                    //Console.WriteLine($"Integration from {curLesserBound} to {curUpperBound} = {threads[curThread].Result}.");
                }));
                threads[curThread].Start();

                //Console.WriteLine($"Spawned thread {curThread} that will integrate from {curLesserBound} to {curUpperBound}.");
            }

            for (int i = 0; i < threads.Length; i++)
            {
                threads[i].Join();
            }

            for (int i = 0; i < threads.Length; i++)
            {
                result += threads[i].Result;
            }

            return result;
        }
    }
}