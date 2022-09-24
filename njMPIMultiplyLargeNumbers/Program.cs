using System.Numerics;

internal class Program
{
    private static void Main(string[] args)
    {
        if (args.Length == 0) args = new string[] { "8" };
        long f = long.Parse(args[0]);



        MPI.Environment.Run(ref args,  comm =>
        {

            long df = f / comm.Size + 1;

            if (df > f) df = f;

            if (comm.Rank == 0)
            {
                BigInteger result = 1;
                long startf = 1;
                long endf = df;
                Console.WriteLine("Process numbers {1} and factorial number for calculating is {2}", comm.Rank, comm.Size, f);

                for (int dest = 1; dest < comm.Size; ++dest)
                {

                    comm.Send(df, dest, 100);
                    BigInteger resulti = comm.Receive<BigInteger>(dest, 100);
                    result *= resulti;
                }
                
                Console.WriteLine("do process with rank 0, calculate factorial from {0} to {1} ", startf, endf);

                for (long k = startf; k <= endf; k++)
                    result *= k;

                Console.WriteLine("result of factorial parallel is {0}", result);

            }
            else
            {
                var dfi = comm.Receive<long>(0, 100);
                BigInteger result = 1;
                long startf = (comm.Rank * dfi)+ 1;
                long endf = (comm.Rank * dfi)+dfi;

                if (endf > f) endf = f;

                Console.WriteLine("do process with rank {0}, calculate factorial from {1} to {2} ", comm.Rank, startf, endf);

                for (long k = startf; k <= endf; k++)
                    result *= k;

                comm.Send(result, 0, 100);
            }
        });

        Console.ReadKey();
    }
}