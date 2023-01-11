using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Text;

namespace SSSTranslator.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public string input { get; set; }
        public int PRIME = 127;

        Random random = new Random();

        public EncryptionModel encryptions = new EncryptionModel();

        public void OnGet()
        {
        }

        public void OnPost() {

            input = Request.Form["input"];

            // System.Diagnostics.Debug.WriteLine(input);
            StringBuilder s1 = new StringBuilder();
            StringBuilder s2 = new StringBuilder();
            StringBuilder s3 = new StringBuilder();


            foreach (int character in input.ToCharArray()) {

                EncryptionModel characterShares = new EncryptionModel();

                characterShares.output = generate_shares(3,2, character);

                s1.Append(characterShares.output[0, 0] + " ");
                s1.Append(characterShares.output[0, 1] + " ");

                s2.Append(characterShares.output[1, 0] + " ");
                s2.Append(characterShares.output[1, 1] + " ");

                s3.Append(characterShares.output[2, 0] + " ");
                s3.Append(characterShares.output[2, 1] + " ");

            }

            this.encryptions.results = s1.ToString() + "\n" + s2.ToString() + "\n" + s3.ToString();

        }

        int polynomial(int x, int[] coeff) {
            
            int point = 0;

            System.Diagnostics.Debug.WriteLine("In polynomial: ");
            foreach (int co in coeff) {
                System.Diagnostics.Debug.Write(co + " ");
            }

            System.Diagnostics.Debug.WriteLine("\n");

            for (int i = coeff.Length - 1; i >= 0; i--) {
                point += ((int)Math.Pow(x, i)) * coeff[i];
            }

            return point % PRIME;
        }

        int[] coefficients(int thresh, int secret){

            List<int> coeffs = new List<int>();

            for (int i = 0; i < thresh - 1; i++) {
                coeffs.Add(random.Next(1, PRIME - 1));
            }

            coeffs.Add(secret);

            return coeffs.ToArray();
        }

        int[,] generate_shares(int num_shares, int threshold, int secret) {

            List<int[]> shares = new List<int[]>();

            int[] coeffs = new int[num_shares - 1];


            Array.Copy(coefficients(threshold, secret), coeffs, coeffs.Length);

            for (int i = 1; i < num_shares+1; i++) {
                int x = random.Next(threshold, secret);
                shares.Add(new int[2] { x, polynomial(x, coeffs)} );
            }

            int[,] shares2d = new int[3, 2];

            for (int i = 0; i < 3; i++) {
                for (int j = 0; j < 2; j++) {
                    shares2d[i, j] = shares[i][j];
                }
            }

            return shares2d;
;
        }
    }

    public class EncryptionModel {
        public int[,]? output { get; set; }
        public string? results { get; set; }
    }
}