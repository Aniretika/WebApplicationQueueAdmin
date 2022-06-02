export interface IPosition {
    id: string;
    authorId: string;
    sourceId: string;
    number: number;
    registrationTime: Date | null;
    requester: string;
    description: string;
}
