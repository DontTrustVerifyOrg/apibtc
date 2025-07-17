import * as $protobuf from "protobufjs";
import Long = require("long");
/** Namespace apibtc. */
export namespace apibtc {

    /** Properties of a UUID. */
    interface IUUID {

        /** UUID Value */
        Value?: (Uint8Array|null);
    }

    /** Represents a UUID. */
    class UUID implements IUUID {

        /**
         * Constructs a new UUID.
         * @param [properties] Properties to set
         */
        constructor(properties?: apibtc.IUUID);

        /** UUID Value. */
        public Value: Uint8Array;

        /**
         * Creates a new UUID instance using the specified properties.
         * @param [properties] Properties to set
         * @returns UUID instance
         */
        public static create(properties?: apibtc.IUUID): apibtc.UUID;

        /**
         * Encodes the specified UUID message. Does not implicitly {@link apibtc.UUID.verify|verify} messages.
         * @param message UUID message or plain object to encode
         * @param [writer] Writer to encode to
         * @returns Writer
         */
        public static encode(message: apibtc.IUUID, writer?: $protobuf.Writer): $protobuf.Writer;

        /**
         * Encodes the specified UUID message, length delimited. Does not implicitly {@link apibtc.UUID.verify|verify} messages.
         * @param message UUID message or plain object to encode
         * @param [writer] Writer to encode to
         * @returns Writer
         */
        public static encodeDelimited(message: apibtc.IUUID, writer?: $protobuf.Writer): $protobuf.Writer;

        /**
         * Decodes a UUID message from the specified reader or buffer.
         * @param reader Reader or buffer to decode from
         * @param [length] Message length if known beforehand
         * @returns UUID
         * @throws {Error} If the payload is not a reader or valid buffer
         * @throws {$protobuf.util.ProtocolError} If required fields are missing
         */
        public static decode(reader: ($protobuf.Reader|Uint8Array), length?: number): apibtc.UUID;

        /**
         * Decodes a UUID message from the specified reader or buffer, length delimited.
         * @param reader Reader or buffer to decode from
         * @returns UUID
         * @throws {Error} If the payload is not a reader or valid buffer
         * @throws {$protobuf.util.ProtocolError} If required fields are missing
         */
        public static decodeDelimited(reader: ($protobuf.Reader|Uint8Array)): apibtc.UUID;

        /**
         * Verifies a UUID message.
         * @param message Plain object to verify
         * @returns `null` if valid, otherwise the reason why it is not
         */
        public static verify(message: { [k: string]: any }): (string|null);

        /**
         * Creates a UUID message from a plain object. Also converts values to their respective internal types.
         * @param object Plain object
         * @returns UUID
         */
        public static fromObject(object: { [k: string]: any }): apibtc.UUID;

        /**
         * Creates a plain object from a UUID message. Also converts values to other types if specified.
         * @param message UUID
         * @param [options] Conversion options
         * @returns Plain object
         */
        public static toObject(message: apibtc.UUID, options?: $protobuf.IConversionOptions): { [k: string]: any };

        /**
         * Converts this UUID to JSON.
         * @returns JSON object
         */
        public toJSON(): { [k: string]: any };

        /**
         * Gets the default type url for UUID
         * @param [typeUrlPrefix] your custom typeUrlPrefix(default "type.googleapis.com")
         * @returns The default type url
         */
        public static getTypeUrl(typeUrlPrefix?: string): string;
    }

    /** Properties of a Timestamp. */
    interface ITimestamp {

        /** Timestamp Value */
        Value?: (number|Long|null);
    }

    /** Represents a Timestamp. */
    class Timestamp implements ITimestamp {

        /**
         * Constructs a new Timestamp.
         * @param [properties] Properties to set
         */
        constructor(properties?: apibtc.ITimestamp);

        /** Timestamp Value. */
        public Value: (number|Long);

        /**
         * Creates a new Timestamp instance using the specified properties.
         * @param [properties] Properties to set
         * @returns Timestamp instance
         */
        public static create(properties?: apibtc.ITimestamp): apibtc.Timestamp;

        /**
         * Encodes the specified Timestamp message. Does not implicitly {@link apibtc.Timestamp.verify|verify} messages.
         * @param message Timestamp message or plain object to encode
         * @param [writer] Writer to encode to
         * @returns Writer
         */
        public static encode(message: apibtc.ITimestamp, writer?: $protobuf.Writer): $protobuf.Writer;

        /**
         * Encodes the specified Timestamp message, length delimited. Does not implicitly {@link apibtc.Timestamp.verify|verify} messages.
         * @param message Timestamp message or plain object to encode
         * @param [writer] Writer to encode to
         * @returns Writer
         */
        public static encodeDelimited(message: apibtc.ITimestamp, writer?: $protobuf.Writer): $protobuf.Writer;

        /**
         * Decodes a Timestamp message from the specified reader or buffer.
         * @param reader Reader or buffer to decode from
         * @param [length] Message length if known beforehand
         * @returns Timestamp
         * @throws {Error} If the payload is not a reader or valid buffer
         * @throws {$protobuf.util.ProtocolError} If required fields are missing
         */
        public static decode(reader: ($protobuf.Reader|Uint8Array), length?: number): apibtc.Timestamp;

        /**
         * Decodes a Timestamp message from the specified reader or buffer, length delimited.
         * @param reader Reader or buffer to decode from
         * @returns Timestamp
         * @throws {Error} If the payload is not a reader or valid buffer
         * @throws {$protobuf.util.ProtocolError} If required fields are missing
         */
        public static decodeDelimited(reader: ($protobuf.Reader|Uint8Array)): apibtc.Timestamp;

        /**
         * Verifies a Timestamp message.
         * @param message Plain object to verify
         * @returns `null` if valid, otherwise the reason why it is not
         */
        public static verify(message: { [k: string]: any }): (string|null);

        /**
         * Creates a Timestamp message from a plain object. Also converts values to their respective internal types.
         * @param object Plain object
         * @returns Timestamp
         */
        public static fromObject(object: { [k: string]: any }): apibtc.Timestamp;

        /**
         * Creates a plain object from a Timestamp message. Also converts values to other types if specified.
         * @param message Timestamp
         * @param [options] Conversion options
         * @returns Plain object
         */
        public static toObject(message: apibtc.Timestamp, options?: $protobuf.IConversionOptions): { [k: string]: any };

        /**
         * Converts this Timestamp to JSON.
         * @returns JSON object
         */
        public toJSON(): { [k: string]: any };

        /**
         * Gets the default type url for Timestamp
         * @param [typeUrlPrefix] your custom typeUrlPrefix(default "type.googleapis.com")
         * @returns The default type url
         */
        public static getTypeUrl(typeUrlPrefix?: string): string;
    }

    /** Properties of a Signature. */
    interface ISignature {

        /** Signature Value */
        Value?: (Uint8Array|null);
    }

    /** Represents a Signature. */
    class Signature implements ISignature {

        /**
         * Constructs a new Signature.
         * @param [properties] Properties to set
         */
        constructor(properties?: apibtc.ISignature);

        /** Signature Value. */
        public Value: Uint8Array;

        /**
         * Creates a new Signature instance using the specified properties.
         * @param [properties] Properties to set
         * @returns Signature instance
         */
        public static create(properties?: apibtc.ISignature): apibtc.Signature;

        /**
         * Encodes the specified Signature message. Does not implicitly {@link apibtc.Signature.verify|verify} messages.
         * @param message Signature message or plain object to encode
         * @param [writer] Writer to encode to
         * @returns Writer
         */
        public static encode(message: apibtc.ISignature, writer?: $protobuf.Writer): $protobuf.Writer;

        /**
         * Encodes the specified Signature message, length delimited. Does not implicitly {@link apibtc.Signature.verify|verify} messages.
         * @param message Signature message or plain object to encode
         * @param [writer] Writer to encode to
         * @returns Writer
         */
        public static encodeDelimited(message: apibtc.ISignature, writer?: $protobuf.Writer): $protobuf.Writer;

        /**
         * Decodes a Signature message from the specified reader or buffer.
         * @param reader Reader or buffer to decode from
         * @param [length] Message length if known beforehand
         * @returns Signature
         * @throws {Error} If the payload is not a reader or valid buffer
         * @throws {$protobuf.util.ProtocolError} If required fields are missing
         */
        public static decode(reader: ($protobuf.Reader|Uint8Array), length?: number): apibtc.Signature;

        /**
         * Decodes a Signature message from the specified reader or buffer, length delimited.
         * @param reader Reader or buffer to decode from
         * @returns Signature
         * @throws {Error} If the payload is not a reader or valid buffer
         * @throws {$protobuf.util.ProtocolError} If required fields are missing
         */
        public static decodeDelimited(reader: ($protobuf.Reader|Uint8Array)): apibtc.Signature;

        /**
         * Verifies a Signature message.
         * @param message Plain object to verify
         * @returns `null` if valid, otherwise the reason why it is not
         */
        public static verify(message: { [k: string]: any }): (string|null);

        /**
         * Creates a Signature message from a plain object. Also converts values to their respective internal types.
         * @param object Plain object
         * @returns Signature
         */
        public static fromObject(object: { [k: string]: any }): apibtc.Signature;

        /**
         * Creates a plain object from a Signature message. Also converts values to other types if specified.
         * @param message Signature
         * @param [options] Conversion options
         * @returns Plain object
         */
        public static toObject(message: apibtc.Signature, options?: $protobuf.IConversionOptions): { [k: string]: any };

        /**
         * Converts this Signature to JSON.
         * @returns JSON object
         */
        public toJSON(): { [k: string]: any };

        /**
         * Gets the default type url for Signature
         * @param [typeUrlPrefix] your custom typeUrlPrefix(default "type.googleapis.com")
         * @returns The default type url
         */
        public static getTypeUrl(typeUrlPrefix?: string): string;
    }

    /** Properties of a PublicKey. */
    interface IPublicKey {

        /** PublicKey Value */
        Value?: (Uint8Array|null);
    }

    /** Represents a PublicKey. */
    class PublicKey implements IPublicKey {

        /**
         * Constructs a new PublicKey.
         * @param [properties] Properties to set
         */
        constructor(properties?: apibtc.IPublicKey);

        /** PublicKey Value. */
        public Value: Uint8Array;

        /**
         * Creates a new PublicKey instance using the specified properties.
         * @param [properties] Properties to set
         * @returns PublicKey instance
         */
        public static create(properties?: apibtc.IPublicKey): apibtc.PublicKey;

        /**
         * Encodes the specified PublicKey message. Does not implicitly {@link apibtc.PublicKey.verify|verify} messages.
         * @param message PublicKey message or plain object to encode
         * @param [writer] Writer to encode to
         * @returns Writer
         */
        public static encode(message: apibtc.IPublicKey, writer?: $protobuf.Writer): $protobuf.Writer;

        /**
         * Encodes the specified PublicKey message, length delimited. Does not implicitly {@link apibtc.PublicKey.verify|verify} messages.
         * @param message PublicKey message or plain object to encode
         * @param [writer] Writer to encode to
         * @returns Writer
         */
        public static encodeDelimited(message: apibtc.IPublicKey, writer?: $protobuf.Writer): $protobuf.Writer;

        /**
         * Decodes a PublicKey message from the specified reader or buffer.
         * @param reader Reader or buffer to decode from
         * @param [length] Message length if known beforehand
         * @returns PublicKey
         * @throws {Error} If the payload is not a reader or valid buffer
         * @throws {$protobuf.util.ProtocolError} If required fields are missing
         */
        public static decode(reader: ($protobuf.Reader|Uint8Array), length?: number): apibtc.PublicKey;

        /**
         * Decodes a PublicKey message from the specified reader or buffer, length delimited.
         * @param reader Reader or buffer to decode from
         * @returns PublicKey
         * @throws {Error} If the payload is not a reader or valid buffer
         * @throws {$protobuf.util.ProtocolError} If required fields are missing
         */
        public static decodeDelimited(reader: ($protobuf.Reader|Uint8Array)): apibtc.PublicKey;

        /**
         * Verifies a PublicKey message.
         * @param message Plain object to verify
         * @returns `null` if valid, otherwise the reason why it is not
         */
        public static verify(message: { [k: string]: any }): (string|null);

        /**
         * Creates a PublicKey message from a plain object. Also converts values to their respective internal types.
         * @param object Plain object
         * @returns PublicKey
         */
        public static fromObject(object: { [k: string]: any }): apibtc.PublicKey;

        /**
         * Creates a plain object from a PublicKey message. Also converts values to other types if specified.
         * @param message PublicKey
         * @param [options] Conversion options
         * @returns Plain object
         */
        public static toObject(message: apibtc.PublicKey, options?: $protobuf.IConversionOptions): { [k: string]: any };

        /**
         * Converts this PublicKey to JSON.
         * @returns JSON object
         */
        public toJSON(): { [k: string]: any };

        /**
         * Gets the default type url for PublicKey
         * @param [typeUrlPrefix] your custom typeUrlPrefix(default "type.googleapis.com")
         * @returns The default type url
         */
        public static getTypeUrl(typeUrlPrefix?: string): string;
    }

    /** Properties of an AuthTokenHeader. */
    interface IAuthTokenHeader {

        /** AuthTokenHeader TokenId */
        TokenId?: (apibtc.IUUID|null);

        /** AuthTokenHeader PublicKey */
        PublicKey?: (apibtc.IPublicKey|null);

        /** AuthTokenHeader Timestamp */
        Timestamp?: (apibtc.ITimestamp|null);
    }

    /** Represents an AuthTokenHeader. */
    class AuthTokenHeader implements IAuthTokenHeader {

        /**
         * Constructs a new AuthTokenHeader.
         * @param [properties] Properties to set
         */
        constructor(properties?: apibtc.IAuthTokenHeader);

        /** AuthTokenHeader TokenId. */
        public TokenId?: (apibtc.IUUID|null);

        /** AuthTokenHeader PublicKey. */
        public PublicKey?: (apibtc.IPublicKey|null);

        /** AuthTokenHeader Timestamp. */
        public Timestamp?: (apibtc.ITimestamp|null);

        /**
         * Creates a new AuthTokenHeader instance using the specified properties.
         * @param [properties] Properties to set
         * @returns AuthTokenHeader instance
         */
        public static create(properties?: apibtc.IAuthTokenHeader): apibtc.AuthTokenHeader;

        /**
         * Encodes the specified AuthTokenHeader message. Does not implicitly {@link apibtc.AuthTokenHeader.verify|verify} messages.
         * @param message AuthTokenHeader message or plain object to encode
         * @param [writer] Writer to encode to
         * @returns Writer
         */
        public static encode(message: apibtc.IAuthTokenHeader, writer?: $protobuf.Writer): $protobuf.Writer;

        /**
         * Encodes the specified AuthTokenHeader message, length delimited. Does not implicitly {@link apibtc.AuthTokenHeader.verify|verify} messages.
         * @param message AuthTokenHeader message or plain object to encode
         * @param [writer] Writer to encode to
         * @returns Writer
         */
        public static encodeDelimited(message: apibtc.IAuthTokenHeader, writer?: $protobuf.Writer): $protobuf.Writer;

        /**
         * Decodes an AuthTokenHeader message from the specified reader or buffer.
         * @param reader Reader or buffer to decode from
         * @param [length] Message length if known beforehand
         * @returns AuthTokenHeader
         * @throws {Error} If the payload is not a reader or valid buffer
         * @throws {$protobuf.util.ProtocolError} If required fields are missing
         */
        public static decode(reader: ($protobuf.Reader|Uint8Array), length?: number): apibtc.AuthTokenHeader;

        /**
         * Decodes an AuthTokenHeader message from the specified reader or buffer, length delimited.
         * @param reader Reader or buffer to decode from
         * @returns AuthTokenHeader
         * @throws {Error} If the payload is not a reader or valid buffer
         * @throws {$protobuf.util.ProtocolError} If required fields are missing
         */
        public static decodeDelimited(reader: ($protobuf.Reader|Uint8Array)): apibtc.AuthTokenHeader;

        /**
         * Verifies an AuthTokenHeader message.
         * @param message Plain object to verify
         * @returns `null` if valid, otherwise the reason why it is not
         */
        public static verify(message: { [k: string]: any }): (string|null);

        /**
         * Creates an AuthTokenHeader message from a plain object. Also converts values to their respective internal types.
         * @param object Plain object
         * @returns AuthTokenHeader
         */
        public static fromObject(object: { [k: string]: any }): apibtc.AuthTokenHeader;

        /**
         * Creates a plain object from an AuthTokenHeader message. Also converts values to other types if specified.
         * @param message AuthTokenHeader
         * @param [options] Conversion options
         * @returns Plain object
         */
        public static toObject(message: apibtc.AuthTokenHeader, options?: $protobuf.IConversionOptions): { [k: string]: any };

        /**
         * Converts this AuthTokenHeader to JSON.
         * @returns JSON object
         */
        public toJSON(): { [k: string]: any };

        /**
         * Gets the default type url for AuthTokenHeader
         * @param [typeUrlPrefix] your custom typeUrlPrefix(default "type.googleapis.com")
         * @returns The default type url
         */
        public static getTypeUrl(typeUrlPrefix?: string): string;
    }

    /** Properties of an AuthToken. */
    interface IAuthToken {

        /** AuthToken Header */
        Header?: (apibtc.IAuthTokenHeader|null);

        /** AuthToken Signature */
        Signature?: (apibtc.ISignature|null);
    }

    /** Represents an AuthToken. */
    class AuthToken implements IAuthToken {

        /**
         * Constructs a new AuthToken.
         * @param [properties] Properties to set
         */
        constructor(properties?: apibtc.IAuthToken);

        /** AuthToken Header. */
        public Header?: (apibtc.IAuthTokenHeader|null);

        /** AuthToken Signature. */
        public Signature?: (apibtc.ISignature|null);

        /**
         * Creates a new AuthToken instance using the specified properties.
         * @param [properties] Properties to set
         * @returns AuthToken instance
         */
        public static create(properties?: apibtc.IAuthToken): apibtc.AuthToken;

        /**
         * Encodes the specified AuthToken message. Does not implicitly {@link apibtc.AuthToken.verify|verify} messages.
         * @param message AuthToken message or plain object to encode
         * @param [writer] Writer to encode to
         * @returns Writer
         */
        public static encode(message: apibtc.IAuthToken, writer?: $protobuf.Writer): $protobuf.Writer;

        /**
         * Encodes the specified AuthToken message, length delimited. Does not implicitly {@link apibtc.AuthToken.verify|verify} messages.
         * @param message AuthToken message or plain object to encode
         * @param [writer] Writer to encode to
         * @returns Writer
         */
        public static encodeDelimited(message: apibtc.IAuthToken, writer?: $protobuf.Writer): $protobuf.Writer;

        /**
         * Decodes an AuthToken message from the specified reader or buffer.
         * @param reader Reader or buffer to decode from
         * @param [length] Message length if known beforehand
         * @returns AuthToken
         * @throws {Error} If the payload is not a reader or valid buffer
         * @throws {$protobuf.util.ProtocolError} If required fields are missing
         */
        public static decode(reader: ($protobuf.Reader|Uint8Array), length?: number): apibtc.AuthToken;

        /**
         * Decodes an AuthToken message from the specified reader or buffer, length delimited.
         * @param reader Reader or buffer to decode from
         * @returns AuthToken
         * @throws {Error} If the payload is not a reader or valid buffer
         * @throws {$protobuf.util.ProtocolError} If required fields are missing
         */
        public static decodeDelimited(reader: ($protobuf.Reader|Uint8Array)): apibtc.AuthToken;

        /**
         * Verifies an AuthToken message.
         * @param message Plain object to verify
         * @returns `null` if valid, otherwise the reason why it is not
         */
        public static verify(message: { [k: string]: any }): (string|null);

        /**
         * Creates an AuthToken message from a plain object. Also converts values to their respective internal types.
         * @param object Plain object
         * @returns AuthToken
         */
        public static fromObject(object: { [k: string]: any }): apibtc.AuthToken;

        /**
         * Creates a plain object from an AuthToken message. Also converts values to other types if specified.
         * @param message AuthToken
         * @param [options] Conversion options
         * @returns Plain object
         */
        public static toObject(message: apibtc.AuthToken, options?: $protobuf.IConversionOptions): { [k: string]: any };

        /**
         * Converts this AuthToken to JSON.
         * @returns JSON object
         */
        public toJSON(): { [k: string]: any };

        /**
         * Gets the default type url for AuthToken
         * @param [typeUrlPrefix] your custom typeUrlPrefix(default "type.googleapis.com")
         * @returns The default type url
         */
        public static getTypeUrl(typeUrlPrefix?: string): string;
    }
}
