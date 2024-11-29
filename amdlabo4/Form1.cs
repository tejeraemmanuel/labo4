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
            dataGridView1.CellClick += dataGridView1_CellClick;
        }

        // Evento para agregar un nuevo alumno
        private void button1_Click(object sender, EventArgs e)
        {
            // Guarda los datos en el archivo
            GrabarDatos();

            // Agrega los datos al DataGridView
            dataGridView1.Rows.Add(txtId.Text, txtNombre.Text, txtApellido.Text, txtDni.Text, dtpFechaNacimiento.Value);

            // Limpia los campos de entrada
            txtId.Text = "";
            txtNombre.Text = "";
            txtApellido.Text = "";
            txtDni.Text = "";
            dtpFechaNacimiento.Value = DateTime.Now;
        }

        private void ActualizarArchivo()
        {
            using (StreamWriter archivo = new StreamWriter("alumnos.txt"))
            {
                foreach (DataGridViewRow fila in dataGridView1.Rows)
                {
                    // Ignorar filas vacías (como la última fila de edición en DataGridView)
                    if (fila.IsNewRow) continue;

                    archivo.WriteLine(fila.Cells[0].Value?.ToString() ?? ""); // ID
                    archivo.WriteLine(fila.Cells[1].Value?.ToString() ?? ""); // Nombre
                    archivo.WriteLine(fila.Cells[2].Value?.ToString() ?? ""); // Apellido
                    archivo.WriteLine(fila.Cells[3].Value?.ToString() ?? ""); // DNI
                    archivo.WriteLine(fila.Cells[4].Value?.ToString() ?? ""); // Fecha Nacimiento
                }
            }
        }

        // Método para guardar datos en el archivo
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

        // Evento que se ejecuta al cargar el formulario
        private void Form1_Load(object sender, EventArgs e)
        {
            // Verifica si el archivo existe, si no, lo crea vacío
            if (!File.Exists("alumnos.txt"))
            {
                using (StreamWriter archivo = new StreamWriter("alumnos.txt"))
                {
                    // Archivo creado vacío
                }
            }
            else
            {
                // Lee los datos del archivo y los carga en el DataGridView
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

        // Evento para eliminar un alumno por ID
        private void button3_Click(object sender, EventArgs e)
        {
            // Recorre las filas del DataGridView
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                // Verifica si el ID ingresado coincide con el de la fila actual
                if (txtId.Text == dataGridView1.Rows[i].Cells[0].Value.ToString())
                {
                    // Elimina la fila del DataGridView
                    dataGridView1.Rows.RemoveAt(i);

                    // Actualiza el archivo con los datos restantes
                    GrabarBorrado();

                    // Muestra un mensaje de confirmación
                    MessageBox.Show("Se borró el alumno.");
                    break; 
                }
            }
        }

        // Método para reescribir el archivo después de un borrado Recorre las filas 
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

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Verifica que el índice de la fila sea válido
            if (e.RowIndex >= 0)
            {
                // Obtén la fila seleccionada
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                // Asigna los valores de las celdas a los TextBox
                txtId.Text = row.Cells["ID"].Value?.ToString();
                txtNombre.Text = row.Cells["NOMBRE"].Value?.ToString();
                txtApellido.Text = row.Cells["APELLIDO"].Value?.ToString();
                txtDni.Text = row.Cells["DNI"].Value?.ToString();
                dtpFechaNacimiento.Value = row.Cells["FechaNacimiento"].Value != null
            ? Convert.ToDateTime(row.Cells["FechaNacimiento"].Value)
            : DateTime.Now;

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Verificar que hay una fila seleccionada
            if (dataGridView1.SelectedRows.Count > 0)
            {
                try
                {
                    // Obtener la fila seleccionada
                    DataGridViewRow filaSeleccionada = dataGridView1.SelectedRows[0];

                    // Actualizar los datos de la fila seleccionada con los valores de los campos de texto
                    filaSeleccionada.Cells[0].Value = txtId.Text;
                    filaSeleccionada.Cells[1].Value = txtNombre.Text;
                    filaSeleccionada.Cells[2].Value = txtApellido.Text;
                    filaSeleccionada.Cells[3].Value = txtDni.Text;
                    filaSeleccionada.Cells[4].Value = dtpFechaNacimiento.Value.ToString();

                    // Actualizar el archivo de texto
                    ActualizarArchivo();

                    MessageBox.Show("Registro modificado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ocurrió un error al modificar el registro: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecciona una fila para modificar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
