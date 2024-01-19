using System;
using System.Security.Cryptography;


struct S3 {
public int X;
public void QuadrupleX() {
X *= 4;
}
}

class Program {
    static void Main() {
        S3? s = new S3 { X = 10 };
        Console.WriteLine(sizeof(S3));
    }
}

