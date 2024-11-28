using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace amdlabo4
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GrabarDatos();
            dataGridView1.Rows.Add(txtId.Text, txtNombre.Text, txtApellido.Text, txtDni.Text, dtpFechaNacimiento.Value);
            txtId.Text = "";
            txtNombre.Text = "";
            txtApellido.Text = "";
            txtDni.Text = "";
            dtpFechaNacimiento.Value = DateTime.Now;
        }

        private void GrabarDatos()
        {
            using (StreamWriter archivo = new StreamWriter("alumnos.txt", true))
            {
                archivo.WriteLine(txtId.Text);
                archivo.WriteLine(txtNombre.Text);
                archivo.WriteLine(txtApellido.Text);
                archivo.WriteLine(txtDni.Text);
                archivo.WriteLine(dtpFechaNacimiento.Value);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!File.Exists("alumnos.txt"))
            {
                using (StreamWriter archivo = new StreamWriter("alumnos.txt"))
                {
                    // Archivo creado vacío
                }
            }
            else
            {
                using (StreamReader archivo = new StreamReader("alumnos.txt"))
                {
                    while (!archivo.EndOfStream)
                    {
                        string id = archivo.ReadLine();
                        string nombre = archivo.ReadLine();
                        string apellido = archivo.ReadLine();
                        string dni = archivo.ReadLine();
                        string fechaNacimiento = archivo.ReadLine();
                        dataGridView1.Rows.Add(id, nombre, apellido, dni, fechaNacimiento);
                    }
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (txtId.Text == dataGridView1.Rows[i].Cells[0].Value.ToString())
                {
                    dataGridView1.Rows.RemoveAt(i);
                    GrabarBorrado();
                    MessageBox.Show("Se borró el alumno.");
                    break;
                }
            }
        }

        private void GrabarBorrado()
        {
            using (StreamWriter archivo = new StreamWriter("alumnos.txt"))
            {
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    archivo.WriteLine(dataGridView1.Rows[i].Cells[0].Value.ToString());
                    archivo.WriteLine(dataGridView1.Rows[i].Cells[1].Value.ToString());
                    archivo.WriteLine(dataGridView1.Rows[i].Cells[2].Value.ToString());
                    archivo.WriteLine(dataGridView1.Rows[i].Cells[3].Value.ToString());
                    archivo.WriteLine(dataGridView1.Rows[i].Cells[4].Value.ToString());
                }
            }
        }

        // Evento para manejar el clic en el DataGridView
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Asegura que el índice de la fila sea válido
            {
                // Obtiene la fila seleccionada
                DataGridViewRow filaSeleccionada = dataGridView1.Rows[e.RowIndex];

                // Asigna los valores de la fila a los campos de entrada
                txtId.Text = filaSeleccionada.Cells[0].Value?.ToString() ?? "";
                txtNombre.Text = filaSeleccionada.Cells[1].Value?.ToString() ?? "";
                txtApellido.Text = filaSeleccionada.Cells[2].Value?.ToString() ?? "";
                txtDni.Text = filaSeleccionada.Cells[3].Value?.ToString() ?? "";

                if (DateTime.TryParse(filaSeleccionada.Cells[4].Value?.ToString(), out DateTime fecha))
                {
                    dtpFechaNacimiento.Value = fecha;
                }
                else
                {
                    dtpFechaNacimiento.Value = DateTime.Now;
                }
            }
        }
    }
}
