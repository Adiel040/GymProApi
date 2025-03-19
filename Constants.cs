using System;

namespace GymProApi
{
	public static class Constants
	{
		public static enum ROLES
		{
			CLIENTE = "Cliente",
			ENTRENADOR = "Entrenador"
		}

		public static enum CATEGORIAS
		{
			SUPLEMENTOS = "Suplemento",
			ACCESORIOS = "Accesorio",
			ROPA = "Ropa"
		}
		
		public static enum RANGOS
		{
			PRINCIPIANTE = "Principiante",
			INTERMEDIO = "Intermedio",
			PROFESIONAL = "Profesional",
			MASTER = "Master"
		}
	}
}
