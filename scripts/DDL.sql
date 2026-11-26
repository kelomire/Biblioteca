CREATE TABLE IF NOT EXISTS personas (
    id           INTEGER PRIMARY KEY AUTOINCREMENT,
    nombre       TEXT NOT NULL,
    apellido     TEXT NOT NULL,
    tipo_persona TEXT NOT NULL
                 CHECK (tipo_persona IN ('ALUMNO','PROFESOR','PRECEPTOR','DIRECTIVO','PERSONAL')),
    curso        TEXT NULL,
    materia      TEXT NULL,
    cargo        TEXT NULL,
    activo       INTEGER NOT NULL DEFAULT 1
);

CREATE TABLE IF NOT EXISTS libros (
    id           INTEGER PRIMARY KEY AUTOINCREMENT,
    codigo_barra TEXT NOT NULL UNIQUE,
    titulo       TEXT NOT NULL,
    autor        TEXT NOT NULL,
    editorial    TEXT NOT NULL,
    anio         INTEGER NOT NULL,
    estado       TEXT NOT NULL DEFAULT 'DISPONIBLE'
                 CHECK (estado IN ('DISPONIBLE','PRESTADO','BAJA'))
);

CREATE TABLE IF NOT EXISTS prestamos (
    id               INTEGER PRIMARY KEY AUTOINCREMENT,
    persona_id       INTEGER NOT NULL REFERENCES personas(id),
    libro_id         INTEGER NOT NULL REFERENCES libros(id),
    fecha_prestamo   TEXT NOT NULL,
    fecha_devolucion TEXT NULL,
    estado           TEXT NOT NULL DEFAULT 'PRESTADO'
                     CHECK (estado IN ('PRESTADO','DEVUELTO'))
);
