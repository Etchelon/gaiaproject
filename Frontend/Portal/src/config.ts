console.log({ meta: import.meta });
export const BASE_URL = import.meta.env.VITE_API_BASE_URL;
// export const BASE_URL = import.meta.env.NODE_ENV === "production" ? import.meta.env.VITE_API_BASE_URL! : "http://<hostname>:5000";
