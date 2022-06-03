import { push } from "svelte-spa-router";
import type { Nullable } from "./miscellanea";

const OK = 200;
const NO_CONTENT = 204;
const UNAUTHENTICATED = 401;
const UNAUTHORIZED = 403;

type HttpVerb = "GET" | "POST" | "PUT" | "DELETE" | "OPTIONS";
export type BearerTokenFactoryFn = () => Promise<Nullable<string>>;
export interface FetchAdditionalOptions {
	contentType?: string;
	readAsString?: boolean;
}

export class HttpClient {
	constructor(private readonly baseUrl: string) {}

	private _bearerTokenFactory: Nullable<BearerTokenFactoryFn> = null;

	setBearerTokenFactory(factory: BearerTokenFactoryFn): void {
		this._bearerTokenFactory = factory;
	}

	private async fetchOptions(verb: HttpVerb, body?: unknown, additionalOptions?: FetchAdditionalOptions): Promise<RequestInit> {
		const token = await this._bearerTokenFactory?.();
		const authHeader = token ? { Authorization: `Bearer ${token}` } : undefined;
		const headers = {
			"Content-Type": additionalOptions?.contentType ?? "application/json",
			...authHeader,
		};
		return {
			method: verb,
			...(body ? { body: JSON.stringify(body) } : undefined),
			cache: "no-cache",
			mode: "cors",
			headers,
		};
	}

	private async checkedFetch<T>(url: string, options: RequestInit, readAsString?: boolean): Promise<T> {
		const response = await fetch(`${this.baseUrl}/${url}`, options);
		switch (response.status) {
			case OK:
				var ret = readAsString ? await response.text() : await response.json();
				return ret;
			case NO_CONTENT:
				return null as any;
			case UNAUTHENTICATED:
			case UNAUTHORIZED:
				push("/unauthorized");
				return {} as any;
			default:
				throw new Error(`Status ${response.status} (${response.statusText}) not handled.`);
		}
	}

	async get<T = void>(url: string, additionalOptions?: FetchAdditionalOptions): Promise<T> {
		const options = await this.fetchOptions("GET", undefined, additionalOptions);
		return await this.checkedFetch<T>(url, options, additionalOptions?.readAsString);
	}

	async post<T = void>(url: string, body?: unknown, additionalOptions?: FetchAdditionalOptions): Promise<T> {
		const options = await this.fetchOptions("POST", body, additionalOptions);
		return await this.checkedFetch<T>(url, options, additionalOptions?.readAsString);
	}

	async put<T = void>(url: string, body?: unknown, additionalOptions?: FetchAdditionalOptions): Promise<T> {
		const options = await this.fetchOptions("PUT", body, additionalOptions);
		return await this.checkedFetch<T>(url, options, additionalOptions?.readAsString);
	}

	async delete<T = void>(url: string, additionalOptions?: FetchAdditionalOptions): Promise<T> {
		const options = await this.fetchOptions("DELETE", undefined, additionalOptions);
		return await this.checkedFetch<T>(url, options, additionalOptions?.readAsString);
	}
}

const actualBaseUrl = import.meta.env.VITE_API_BASE_URL;
const httpClient = new HttpClient(actualBaseUrl);
export default httpClient;
