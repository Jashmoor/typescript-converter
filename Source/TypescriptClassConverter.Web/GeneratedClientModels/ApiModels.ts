export namespace ApiModels {
	export enum Status {
		Good = 0,
		Ok = 1,
		Bad = 2,
	}

	export interface WeatherForecast {
		Date : string | Date;
		TemperatureC : number;
		TemperatureF : number;
		Summary : string;
	}
	export interface Base<T> {
		Test : T;
	}
	export interface TestResponse extends Base<number> {
		ResponseId : string;
		ResponseDate : string | Date;
	}
	export interface IRequest<T> {
		Body : T;
	}
	export interface TestRequest extends IRequest<string> {
		Status : Status;
		Body : string;
		Id : number;
	}
	export interface TestOtherRequest extends TestRequest {
		Working : boolean;
		Object : any;
	}
	export interface IStructuralEquatable {
	}
	export interface IStructuralComparable {
	}
	export interface IComparable {
	}
	export interface ITuple {
		Length : number;
		Item : any;
	}
	export interface IValueTupleInternal extends ITuple {
	}
	export interface ValueTuple<T1, T2> extends IStructuralEquatable, IStructuralComparable, IComparable, IValueTupleInternal {
		Item1 : T1;
		Item2 : T2;
	}
	export interface EmptyResponse {
		Tuple : ValueTuple<string, number>;
	}
	export interface IEmptyInterface<T> {
	}
	export interface EmptyRequest extends IEmptyInterface<EmptyResponse> {
		Name : string;
		Id : number;
	}
}