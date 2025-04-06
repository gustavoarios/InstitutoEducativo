# Instituto Educativo 📖

## Objetivos 📋
Desarrollar un sistema, que permita la administración básica de un Instituto Educativo de cara a los Empleados con los Profesores, Alumnos, Materias, Cursos, Calificaciones, Carreras, etc., como así también, permitir a los Profesores, realizar calificaciones y a los Alumnos matricularse en las materias pendientes.
Utilizar Visual Studio 2022 community edition y crear una aplicación utilizando ASP.NET MVC Core (versión a definir por el docente, actualmente 8.0).

<hr />

## Enunciado 📢
La idea principal de este trabajo práctico, es que Uds. se comporten como un equipo de desarrollo.
Este documento, les acerca, un equivalente al resultado de una primera entrevista entre el cliente y alguien del equipo, el cual relevó e identificó la información aquí contenida. 
A partir de este momento, deberán comprender lo que se está requiriendo y construir dicha aplicación web.

Lo primero que deben hacer es comprender en detalle, que es lo que se espera y se busca como resultado del proyecto, para ello, deben recopilar todas las dudas que tengan entre Uds. y evacuarlas con su nexo (el docente) de cara al cliente. De esta manera, él nos ayudará a conseguir la información ya un poco más procesada. 
Es importante destacar, que este proceso no debe esperarse hacerlo 100% en clase; deben ir contemplandolas de manera independientemente, las unifican y hace una puesta comun dentro del equipo (ya sean de índole funcional o técnicas), en lugar de enviar consultas individuales, se sugiere y solicita que las envien de manera conjunta. 

Al inicio del proyecto, las consultas pueden ser enviadas por correo siguiente el siguiente formato:

Subject: [NT1-<CURSO LETRA>-GRP-<GRUPO NUMERO>] <Proyecto XXX> | Informativo o Consulta

Body: 

1.<xxxxxxxx>
2.< xxxxxxxx>

# Ejemplo
**Subject:** [NT1-A-GRP-5] Agenda de Turnos | Consulta

**Body:**

1.La relación del paciente con Turno es 1:1 o 1:N?
2.Está bien que encaremos la validación del turno activo, con una propiedad booleana en el Turno?

<hr />

Es sumamente importante que los correos siempre tengan:
1.Subject con la referencia, para agilizar cualquier interaccion entre el docente y el grupo
2. Siempre que envien una duda o consulta, pongan en copia a todos los participantes del equipo. 

Nota: A medida que avancemos en la materia, TODAS las dudas relacionadas al proyecto deberán ser canalizadas por medio de Github, y desde alli tendremos: seguimiento y las dudas con comentarios, accesibles por todo el equipo y el avance de las mismas. 

**Crear un Issue nuevo o agregar un comentario sobre un issue en cuestion**, si se requiere asistencia, evacuar una duda o lo que fuese, siempre arrobando al docente, ejemplo: @marianolongoort y agregando las etiquetas correspondientes.


### Proceso de ejecución en alto nivel ☑️
 - Crear un nuevo proyecto en [visual studio](https://visualstudio.microsoft.com/en/vs/) utilizando la template de MVC (Model-View-Controller).
 - Crear todos los modelos definidos y/o detectados por ustedes, dentro de la carpeta Models, cada uno en un archivo separado (Modelos anemicos, modelos sin responsabilidades).
 - En el proyecto trataremos de reducir al mínimo las herencias sobre los modelos anémicos.  Ej. la clase Persona, tendrá especializaciones como ser Empleado, Cliente, Alumno, Profesional, etc. según corresponda al proyecto.
 - Sobre dichos modelos, definir y aplica las restricciones necesarias y solicitadas para cada una de las entidades. [DataAnnotations](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations?view=net-8.0).
 - Agregar las propiedades navegacionales que pudisen faltar, para las relaciones entre las entidades (modelos) nueva que pudieramos generar o encontrar.
 - Agregar las propiedades relacionales, en el modelo donde se quiere alojar la relacion (entidad dependiente). La entidad fuerte solo tendrá una propiedad Navegacional, mientras que la entidad debíl tendrá la propiedad relacional.
 - Crear una carpeta Data en la raíz del proyecto, y crear dentro al menos una clase que representará el contexto de la base de datos (DbContext - los datos a almacenar) para nuestra aplicacion. 
 - Agregar los paquetes necesarios para Incorporar Entity Framework e Identitiy en nuestros proyectos.
 - Crear el DbContext utilizando en esta primera estapa con base de datos en memoria (con fines de testing inicial, introduccion y fine tunning de las relaciones entre modelos). [DbContext](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbcontext?view=efcore-8.0), [Database In-Memory](https://docs.microsoft.com/en-us/ef/core/providers/in-memory/?tabs=vs).
 - Agregar las propiedades del tipo DbSet para cada una de las entidades que queremos persistir en el DbContext. Estas propiedades, serán colecciones de tipos que deseamos trabajar en la base de datos. En nuestro caso, serán las Tablas en la base de datos.
 - Agregar Identity a nuestro proyecto, y al menos definir IdentityUser como clase base de Persona en nuestro poryecto. Esto nos facilitará la inclusion de funcionalidades como Iniciar y cerrar sesion, agregado de entidades de soporte para esto Usuario y Roles que nos serviran para aplicar un control de acceso basado en roles (RBAC) basico. 
 - Por medio de Scaffolding, crear en esta instancia todos los CRUD (Create-Read-Update-Delete)/ABM (Altas-Bajas-Modificaiciones) de las entidades a persistir. Luego verificaremos cuales mantenemos, cuales removemos, y cuales adecuaremos para darle forma a nuestra WebApp.
 - Antes de continuar es importante realizar algun tipo de pre-carga de la base de datos. No solo es requisito del proyecto, sino que les ahorrara mucho tiempo en las pruebas y adecuaciones de los ABM.
 - Testear en detalle los ABM generados, y detectar todas las modificaciones requeridas para nuestros ABM e interfaces de usuario faltantes para resolver funcionalidades requeridas. (siempre tener presente el checklist de evaluacion final, que les dara el rumbo para esto).
 - Cambiar el dabatabase service provider de Database In Memory a SQL. Para aquellos casos que algunos alumnos utilicen MAC, tendran dos opciones para avanzar (adecuar el proyecto, para utilizar SQLLite o usar un docker con SQL Server instalado alli).
 - Aplicar las adecuaciones y validaciones necesarias en los controladores.  
 - Si el proyecto lo requiere, generar el proceso de auto-registración. Es importante aclarar que este proceso debe ser adecuado según las necesidades de cada proyecto, sus entidades y requerimientos al momento de auto-registrar; no seguir explicitamente un registro tan simple como email y password. 
 - A estas alturas, ya se han topado con varios inconvenientes en los procesos de adecuacion de las vistas y por consiguiente es una buena idea que generen ViewModels para desbloquear esas problematicas que nos estan trayendo los Modelos anemicos utilizados hasta el momento.
 - En el caso de ser requerido en el enunciado, un administrador podrá realizar todas tareas que impliquen interacción del lado del negocio (ABM "Alta-Baja-Modificación" de las entidades del sistema y configuraciones en caso de ser necesarias).
 - El <Usuario Cliente o equivalente> sólo podrá tomar acción en el sistema, en base al rol que que se le ha asignado al momento de auto-registrarse o creado por otro medio o entidad.
 - Realizar todos los ajustes necesarios en los modelos y/o código desde la perspectiva de funcionalidad.
 - Realizar los ajustes requeridos desde la perspectiva de permisos y validaciones.
 - Realizar los ajustes y mejoras en referencia a la presentación de la aplicaión (cuestiones visuales).
 
 Nota: Para la pre-carga de datos, las cuentas creadas por este proceso, deben cumplir las siguientes reglas de manera EXCLUYENTE:
 1. La contraseña por defecto para todas las cuentas pre-cargadas será: Password1!
 2. El UserName y el Email deben seguir la siguiente regla:  <classname>+<rolname si corresponde diferenciar>+<indice>@ort.edu.ar Ej.: cliente1@ort.edu.ar, empleado1@ort.edu.ar, empleadorrhh1@ort.edu.ar

<hr />

## Entidades 📄
- Persona
- Empleado
- Profesor
- Alumno
- Carrera
- Materia
- MateriaCursada
- Inscripcion
- Calificacion

## `⚠️Importante: Todas las entidades deben tener su identificador único. Id⚠️`

`
Las propiedades descriptas a continuación, son las mínimas que deben tener las entidades. Uds. pueden proponer agregar las que consideren necesarias. Siempre validar primero con el docente.
De la misma manera Uds. deben definir los tipos de datos asociados a cada una de ellas, como así también las restricciones.
`

**Persona**
```
- UserName
- Email
- FechaAlta
- Nombre
- Apellido
- DNI
- Telefono
- Direccion
- Activo
```

**Empleado**
```
- UserName
- Email
- FechaAlta
- Nombre
- Apellido
- DNI
- Telefono
- Direccion
- Activo
- Legajo
```

**Profesor**
```
- UserName
- Email
- FechaAlta
- Nombre
- Apellido
- DNI
- Telefono
- Direccion
- Activo
- Legajo
- MateriasCursada (Designado)
- Calificaciones (Realizadas)
```

**Alumno**
```
- UserName
- Email
- FechaAlta
- Nombre
- Apellido
- DNI
- Telefono
- Direccion
- Activo
- NumeroMatricula
- Carrera
- Inscripciones
- Calificaciones
```

**Carrera**
```
- Nombre
- Materias
- Alumnos
```

**Materia**
```
- Carrera
- Nombre (Programación en nuevas tecnologías 1)
- CodigoMateria (PNT1)
- Descripcion 
- CupoMaximo
- Cursadas 
```

**MateriaCursada**
```
- Materia
- CodigoCursada (A, B, C, etc.)
- Anio (2025)
- Cuatrimestre (1 o 2)
- Nombre (dato calculado: ej. PNT1-2025-1C-C)
- Activo (flag)
- Profesor
- Inscripciones
```

**Calificacion**
```
- Fecha (solo fecha)
- Nota (enum)
- Profesor
- Inscripcion
- Alumno
```

**NOTA:** aquí un link para refrescar el uso de los [Data annotations](https://www.c-sharpcorner.com/UploadFile/af66b7/data-annotations-for-mvc/).

<hr />

## Características y Funcionalidades ⌨️
`⚠️Todas las entidades, deben tener implementado su correspondiente ABM, a menos que sea implícito el no tener que soportar alguna de estas acciones.⚠️`
 
 **NOTA:** En el EP3, se deberán presentar las ABM de todoas las entidades, independientemente de que luego sean modificadas o eliminadas. El fin academico de esto, es que tomen contacto con formas de manejar los datos con los usuarios desde una interfaz gráfica de usuario y les sea más facil en el siguiente entregable comprender que deben modificar o adecuar.

## Generalidades 🏠
- La aplicación deberá incluir un logo en el layout
- Deberá contar con información institucional (inventada) relacionada al proyecto.
- Contenido anónimo que debe estar disponible (sin iniciar sesión):    
    - Se listarán profesores de la institución.
    - Listar carreras y materias por carrera.
    - Los alumnos pueden auto registrarse, pero quedarán inactivos, hasta que un empleado los active.
        - Que un alumno esté inactivo, significa que no podrá matricularse para cursar las materias.
- La autoregistración desde el sitio, es exclusiva pra los alumnos.
- Los profesores y empleados, deben ser agregados por un empleado (No pueden auto registrarse).
	- Al momento, del alta del profesor y/o empleado, se le definirá un username y la password será definida por el sistema.
    - También se le asignará a estas cuentas el rol según corresponda.

**Alumno**
- No puede actualizar datos de contacto, solo puede hacerlo un empleado.
- Puede auto registrarse definiendo también su password, a diferencia si es creado por un empleado de oficina de alumnos.
- Un alumno puede inscribirse para cursar materias de forma Online
    - Puede matricularse en hasta 5 cursadas por cuatrimestre.
    - Las materias en las cuales puede inscribirse, deben ser de la carrera que cursa el alumno y no debe tenerla activa o ya haberla cursado.
- Puede ver las materias que ya cursó con su nota.    
- Puede ver las materias que está inscripto/cursando actualmente.
    - El alumno puede cancelar la inscripción a una materia en cualquier momento, pero no debe tener una calificación asociada. En dicho caso, no podrá darse de baja.
        - Si se da de baja, la inscripción deberá quedar con una calificación que indique Baja.
    - En el detalle de la materia cursada inscripto, el alumno, puede ver un listado de sus compañeros, con Nombre, Apellido y el correo electronico. 

**Empleado**
- Un Empleado, puede crear más empleados, profesores y alumnos.
    - Las contraseñas serán asignadas automaticamente por el sistema.
    - Se definirán los roles correspondientes por cada tipo.
    - No se pueden eliminar empleados, profesores y alumnos, solo deshabilitar.
        - Si los profesores están deshabilitados, no deben aparecer en la oferta de profesores a seleccionar en cursadas, ni en el listado institucional.        
- Puede hacer ABM de Carreras, Materias, MateriasCursadas e Inscripciones.
- No puede eliminar Carreras ni Materias.
- Solo puede eliminar una MateriaCursada, si no tiene inscripciones con calificaciones.
- No puede calificar.
- Solo un empleado puede modificar la asignación de un profesor a una cursada.

**Profesor**
- No puede crear empleados, ni profesores, ni alumnos.
- Puede listar las materias cursadas, que se le han asignado vigentes y pasadas (listados separados).
    - En cada caso podrá ver los alumnos inscriptos.
    - Por cada alumno, podrá realizar una calificación, en tanto y cuanto esté vigente (1er Cuatrimestre Hasta 31 de Julio, 2do Cuatrimestre hasta 31 de Diciembre).
        - Las calificaciones posibles serán del 0 a 10 o Ausente, Pendiente, Baja.
        - Solo el profesor titular de la cursada, podrá hacer la calificación y quedará registro del mismo.
    - Por cada materia cursada, el profesor podrá ver un promedio de las notas de los alumnos.

**Materia y MateriaCursada**
- No existen correlatividades entre las materias.
- La materia debe pertenecer a una carrera.
   - En el caso de que exista una misma materia en más de una carrera deberá crearse una nueva. No hay materias Cross-Carrera, por lo cual, la Materia debe tener una relación con la Carrera y utilizar el CodigoCarrera como DisplayName. Ej. AnSis.Matematica, BioTec.Matematica.
- Las materias tendrán un cupo máximo de alumnos.
    - En caso de que se alcance el cupo máximo de inscripciones en una MateriaCursada, se deberá generar una nueva MateriaCursada (un nuevo curso). Ejemplo Si el limite es 10, y existen 10 alumnos registrados en NT1-2025-1C-A, el alumno 11 al registrarse, se inscribirá en NT1-2025-1C-B.    
- Se asume que los profesores pueden dar más de una materia, no hay restricción de día, horario, carga horaria, etc.
    - Por consiguiente, la asignación del profesor en la creación de un nuevo curso automático, será el mismo, del curso previo. Ej. El profesor del curso A, se le asignará al curso B.
    - Un empleado y solo este, podrá modificar la asignación de un profesor a una materia cursada.


**Aplicación General**
- No se debe poder acceder, ejecutar modificaciones/acciones no permitidas por acceso desde la URL.
- No se pueden repetir/duplicar la combinación de Carrera.Nombre.
