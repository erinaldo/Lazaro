using System;
using System.Collections.Generic;
using System.Text;

namespace Lbl.Entidades
{

        /// <summary>
        /// Representa una Provincia, Departamento o Localidad.
        /// </summary>
        [Lbl.Atributos.Nomenclatura(NombreSingular = "Localidad")]
        [Lbl.Atributos.Datos(TablaDatos = "ciudades", CampoId = "id_ciudad")]
        [Lbl.Atributos.Presentacion()]
	public class Localidad : ElementoDeDatos
	{
                private Localidad m_Parent = null;
                private Pais m_Pais = null;

		//Heredar constructor
		public Localidad(Lfx.Data.IConnection dataBase)
                        : base(dataBase) { }

		public Localidad(Lfx.Data.IConnection dataBase, int itemId)
			: base(dataBase, itemId) { }

                public Localidad(Lfx.Data.IConnection dataBase, Lfx.Data.Row row)
                        : base(dataBase, row) { }


                public string CodigoPostal
                {
                        get
                        {
                                return this.GetFieldValue<string>("cp");
                        }
                        set
                        {
                                this.Registro["cp"] = value;
                        }
                }


                public Localidad Provincia
                {
                        get
                        {
                                if (m_Parent == null && this.Registro["id_provincia"] != null)
                                        m_Parent = new Localidad(this.Connection, this.GetFieldValue<int>("id_provincia"));

                                return m_Parent;
                        }
                        set
                        {
                                m_Parent = value;
                                if (value == null)
                                        this.SetFieldValue("id_provincia", null);
                                else
                                        this.SetFieldValue("id_provincia", value.Id);
                        }
                }


                public Pais Pais
                {
                        get
                        {
                                if (m_Pais == null && this.Registro["id_pais"] != null)
                                        m_Pais = new Pais(this.Connection, this.GetFieldValue<int>("id_pais"));

                                return m_Pais;
                        }
                        set
                        {
                                m_Pais = value;
                                if (value == null)
                                        this.SetFieldValue("id_pais", null);
                                else
                                        this.SetFieldValue("id_pais", value.Id);
                        }
                }


                public Impuestos.SituacionIva ObtenerIva()
                {
                        if (this.GetFieldValue<int>("iva") == 1)
                                return Impuestos.SituacionIva.Exento;
                        else if (this.Provincia != null)
                                return this.Provincia.Iva;
                        else
                                return Impuestos.SituacionIva.Predeterminado;
                }

                public Impuestos.SituacionIva Iva
                {
                        get
                        {
                                return (Impuestos.SituacionIva)(this.GetFieldValue<int>("iva"));
                        }
                        set
                        {
                                this.SetFieldValue("iva", (int)value);
                        }
                }


                public override Lfx.Types.OperationResult Guardar()
                {
                        qGen.IStatement Comando;

                        if (this.Existe == false) {
                                Comando = new qGen.Insert(this.TablaDatos);
                                Comando.ColumnValues.AddWithValue("fecha", new qGen.SqlExpression("NOW()"));
                        } else {
                                Comando = new qGen.Update(this.TablaDatos);
                                Comando.WhereClause = new qGen.Where(this.CampoId, this.Id);
                        }

                        Comando.ColumnValues.AddWithValue("nombre", this.Nombre);
                        Comando.ColumnValues.AddWithValue("cp", this.CodigoPostal);
                        if (this.Provincia == null) {
                                Comando.ColumnValues.AddWithValue("id_provincia", null);
                        } else {
                                Comando.ColumnValues.AddWithValue("id_provincia", this.Provincia.Id);
                        }
                        Comando.ColumnValues.AddWithValue("iva", (int)(this.Iva));

                        this.AgregarTags(Comando);

                        this.Connection.ExecuteNonQuery(Comando);

                        return base.Guardar();
                }
	}
}
