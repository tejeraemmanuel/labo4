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
using System.Runtime.Remoting.Messaging;



namespace amdlabo4
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            dataGridView1.CellClick += dataGridView1_CellClick;

            // Configurar la columna ID como numérica
            dataGridView1.Columns[0].ValueType = typeof(int);
            txtId.Enabled = false;
            
            

        }

        //=========== BOTONES ===========//

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            // Validar que todos los campos tengan datos
            if (!CamposValidos())
            {
                return; // Detener si hay campos vacíos o incorrectos
            }

            // Validar que el DNI no esté duplicado
            if (!DniUnico(txtDni.Text))
            {
                MessageBox.Show("El DNI ingresado ya existe. Por favor, ingresa un DNI único.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validar la fecha
            if (!EsFechaValida(dtpFechaNacimiento.Value))
            {
                return;
            }

            int nuevoId = GenerarIdSiguiente();

            // Agregar los datos al DataGridView y guardar en archivo
            dataGridView1.Rows.Add(nuevoId, txtNombre.Text, txtApellido.Text, txtDni.Text, dtpFechaNacimiento.Value.ToString("yyyy-MM-dd"), txtDomicilio.Text);
            GrabarDatos();
            MessageBox.Show($"El registro se agregó con éxito. El ID asignado es: {nuevoId}", "Registro Agregado", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LimpiarCampos();
        }








        private void button2_Click(object sender, EventArgs e)
        {
            // Validar que todos los campos tengan datos
            if (!CamposValidos())
            {
                return; // Detener si hay campos vacíos o incorrectos
            }

            // Verificar si el DNI es único, pero solo si se ha cambiado
            if (txtDni.Text != dataGridView1.SelectedRows[0].Cells[3].Value?.ToString())
            {
                if (!DniUnico(txtDni.Text))
                {
                    MessageBox.Show("El DNI ingresado ya existe. Por favor, ingresa un DNI único.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Validar el DNI
                if (txtDni.Text.Length != 8 || !txtDni.Text.All(char.IsDigit))
                {
                    MessageBox.Show("El DNI debe tener exactamente 8 dígitos numéricos.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    DataGridViewRow filaSeleccionada = dataGridView1.SelectedRows[0];
                    filaSeleccionada.Cells[1].Value = txtNombre.Text;
                    filaSeleccionada.Cells[2].Value = txtApellido.Text;
                    filaSeleccionada.Cells[3].Value = txtDni.Text;
                    filaSeleccionada.Cells[4].Value = dtpFechaNacimiento.Value.ToString("yyyy-MM-dd");
                    filaSeleccionada.Cells[5].Value = txtDomicilio.Text;

                    GrabarDatos();
                    LimpiarCampos();
                    MessageBox.Show("Registro modificado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ocurrió un error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Selecciona una fila para modificar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }




        private void button3_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (int.TryParse(txtId.Text, out int id) && id == Convert.ToInt32(dataGridView1.Rows[i].Cells[0].Value))
                {
                    dataGridView1.Rows.RemoveAt(i);
                    GrabarDatos();
                    LimpiarCampos();
                    MessageBox.Show("Se borró el alumno.");
                    break;
                }
            }
        }

        //=========== DGRID ===========//
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
                        string idTexto = archivo.ReadLine();
                        if (int.TryParse(idTexto, out int id))
                        {
                            string nombre = archivo.ReadLine();
                            string apellido = archivo.ReadLine();
                            string dni = archivo.ReadLine();
                            string fechaNacimiento = archivo.ReadLine();
                            string domicilio = archivo.ReadLine();

                            dataGridView1.Rows.Add(id, nombre, apellido, dni, fechaNacimiento, domicilio);
                        }
                    }
                }
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow fila = dataGridView1.Rows[e.RowIndex];

                txtId.Text = fila.Cells[0].Value?.ToString();
                txtNombre.Text = fila.Cells[1].Value?.ToString();
                txtApellido.Text = fila.Cells[2].Value?.ToString();
                txtDni.Text = fila.Cells[3].Value?.ToString();
                dtpFechaNacimiento.Value = Convert.ToDateTime(fila.Cells[4].Value);
                txtDomicilio.Text = fila.Cells[5].Value?.ToString();
            }
        }

        //=========== METODOS ===========//
        private int GenerarIdSiguiente()
        {
            // Crea una lista de todos los ID actuales en el DataGridView
            List<int> idsExistentes = new List<int>();

            foreach (DataGridViewRow fila in dataGridView1.Rows)
            {
                if (!fila.IsNewRow)
                {
                    if (fila.Cells[0].Value is int id)
                    {
                        idsExistentes.Add(id);
                    }
                }
            }

            // Ordena la lista para buscar el menor ID disponible
            idsExistentes.Sort();

            // Encontrar el menor ID disponible
            int siguienteId = 1;
            foreach (int id in idsExistentes)
            {
                if (id == siguienteId)
                {
                    siguienteId++; // Avanzar al siguiente numero
                }
                else
                {
                    break; // ID vacio encontrado
                }
            }

            return siguienteId; // Retornar el menor ID disponible
        }

        private void GrabarDatos()
        {
            using (StreamWriter archivo = new StreamWriter("alumnos.txt"))
            {
                foreach (DataGridViewRow fila in dataGridView1.Rows)
                {
                    if (!fila.IsNewRow)
                    {
                        archivo.WriteLine(fila.Cells[0].Value?.ToString()); // ID
                        archivo.WriteLine(fila.Cells[1].Value?.ToString()); // Nombre
                        archivo.WriteLine(fila.Cells[2].Value?.ToString()); // Apellido
                        archivo.WriteLine(fila.Cells[3].Value?.ToString()); // DNI
                        archivo.WriteLine(Convert.ToDateTime(fila.Cells[4].Value).ToString("yyyy-MM-dd")); // Fecha de nacimiento                                                                                                           
                        archivo.WriteLine(fila.Cells[5].Value?.ToString()); // Domicilio
                    }
                }
            }
        }

        private void LimpiarCampos()
        {
            txtNombre.Text = "";
            txtApellido.Text = "";
            txtDni.Text = "";
            dtpFechaNacimiento.Value = DateTime.Now;
            txtDomicilio.Text = "";
            txtId.Text = "";
        }

        private void txtDni_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Permitir solo números del 0 al 9 y teclas de control como backspace
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                MessageBox.Show("Solo se permiten números", "Validación DNI", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handled = true; // Bloquear entrada no válida
                return;
            }

            // Validar longitud máxima de 8 caracteres
            if (char.IsDigit(e.KeyChar) && txtDni.Text.Length >= 8)
            {
                MessageBox.Show("El DNI debe tener exactamente 8 números", "Validación DNI", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Handled = true; // Bloquear más caracteres
            }
        }

        private void txtNombre_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Permitir solo letras, espacio y teclas de control (como backspace)
            if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar) && e.KeyChar != ' ')
            {
                MessageBox.Show("Solo se permiten letras", "Validación Nombre", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Handled = true; // Bloquear entrada no válida
            }
        }
        private bool EsFechaValida(DateTime fecha)
        {
            // Compara solo la fecha
            if (fecha >= DateTime.Today)
            {
                MessageBox.Show("La fecha de nacimiento no puede ser hoy o posterior.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }



        private void txtApellido_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Permitir solo letras, espacio y teclas de control (como backspace)
            if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar) && e.KeyChar != ' ')
            {
                MessageBox.Show("Solo se permiten letras", "Validación Nombre", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Handled = true; // Bloquear entrada no válida
            }
        }


        private bool CamposValidos()
        {
            // Verificar si alguno de los campos está vacío
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El campo Nombre es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtApellido.Text))
            {
                MessageBox.Show("El campo Apellido es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtDni.Text))
            {
                MessageBox.Show("El campo DNI es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (txtDni.Text.Length != 8 || !txtDni.Text.All(char.IsDigit))
            {
                MessageBox.Show("El DNI debe tener exactamente 8 dígitos numéricos.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtDomicilio.Text))
            {
                MessageBox.Show("El campo Domicilio es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (dtpFechaNacimiento.Value >= DateTime.Today)
            {
                MessageBox.Show("La fecha de nacimiento no puede ser hoy o posterior a hoy.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true; // Si todos los campos son válidos
        }



        private bool DniUnico(string dni)
        {
            foreach (DataGridViewRow fila in dataGridView1.Rows)
            {
                // Verifica si el DNI de alguna fila es igual al ingresado
                if (fila.Cells[3].Value?.ToString() == dni)
                {
                    return false; // Si encuentra un duplicado, retorna false
                }
            }
            return true; // Si no hay duplicados, retorna true
        }


    }
}
