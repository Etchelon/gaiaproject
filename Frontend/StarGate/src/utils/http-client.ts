import { BASE_URL } from "../config";
import { Nullable } from "./miscellanea";

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

class HttpClient {
	constructor(private readonly baseUrl: string) {}

	private _bearerTokenFactory: BearerTokenFactoryFn = async () => null;

	setBearerTokenFactory(factory: () => Promise<string>): void {
		this._bearerTokenFactory = factory;
	}

	private async fetchOptions(verb: HttpVerb, body?: unknown, additionalOptions?: FetchAdditionalOptions): Promise<RequestInit> {
		const token = await this._bearerTokenFactory();
		const authHeader = token !== null ? { Authorization: `Bearer ${token}` } : undefined;
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

	private async checkedFetch<T>(url: string, options: RequestInit, readAsString?: boolean): Promise<T | null> {
		const response = await fetch(`${this.baseUrl}/${url}`, options);
		switch (response.status) {
			case OK:
				var ret = readAsString ? await response.text() : await response.json();
				return ret;
			case NO_CONTENT:
				return null;
			case UNAUTHENTICATED:
			case UNAUTHORIZED:
				window.location.assign("/unauthorized");
				return null;
			default:
				throw new Error(`Status ${response.status} (${response.statusText}) not handled.`);
		}
	}

	async get<T = void>(url: string, additionalOptions?: FetchAdditionalOptions): Promise<T | null> {
		const options = await this.fetchOptions("GET", undefined, additionalOptions);
		return await this.checkedFetch<T | null>(url, options, additionalOptions?.readAsString);
	}

	async post<T = void>(url: string, body?: unknown, additionalOptions?: FetchAdditionalOptions): Promise<T | null> {
		const options = await this.fetchOptions("POST", body, additionalOptions);
		return await this.checkedFetch<T | null>(url, options, additionalOptions?.readAsString);
	}

	async put<T = void>(url: string, body?: unknown, additionalOptions?: FetchAdditionalOptions): Promise<T | null> {
		const options = await this.fetchOptions("PUT", body, additionalOptions);
		return await this.checkedFetch<T | null>(url, options, additionalOptions?.readAsString);
	}
}

const actualBaseUrl = BASE_URL.replace("<hostname>", window.location.hostname);
const httpClient = new HttpClient(actualBaseUrl);
export default httpClient;
