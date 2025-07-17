/*eslint-disable block-scoped-var, id-length, no-control-regex, no-magic-numbers, no-prototype-builtins, no-redeclare, no-shadow, no-var, sort-vars*/
import * as $protobuf from "protobufjs/minimal";

// Common aliases
const $Reader = $protobuf.Reader, $Writer = $protobuf.Writer, $util = $protobuf.util;

// Exported root namespace
const $root = $protobuf.roots["default"] || ($protobuf.roots["default"] = {});

export const apibtc = $root.apibtc = (() => {

    /**
     * Namespace apibtc.
     * @exports apibtc
     * @namespace
     */
    const apibtc = {};

    apibtc.UUID = (function() {

        /**
         * Properties of a UUID.
         * @memberof apibtc
         * @interface IUUID
         * @property {Uint8Array|null} [Value] UUID Value
         */

        /**
         * Constructs a new UUID.
         * @memberof apibtc
         * @classdesc Represents a UUID.
         * @implements IUUID
         * @constructor
         * @param {apibtc.IUUID=} [properties] Properties to set
         */
        function UUID(properties) {
            if (properties)
                for (let keys = Object.keys(properties), i = 0; i < keys.length; ++i)
                    if (properties[keys[i]] != null)
                        this[keys[i]] = properties[keys[i]];
        }

        /**
         * UUID Value.
         * @member {Uint8Array} Value
         * @memberof apibtc.UUID
         * @instance
         */
        UUID.prototype.Value = $util.newBuffer([]);

        /**
         * Creates a new UUID instance using the specified properties.
         * @function create
         * @memberof apibtc.UUID
         * @static
         * @param {apibtc.IUUID=} [properties] Properties to set
         * @returns {apibtc.UUID} UUID instance
         */
        UUID.create = function create(properties) {
            return new UUID(properties);
        };

        /**
         * Encodes the specified UUID message. Does not implicitly {@link apibtc.UUID.verify|verify} messages.
         * @function encode
         * @memberof apibtc.UUID
         * @static
         * @param {apibtc.IUUID} message UUID message or plain object to encode
         * @param {$protobuf.Writer} [writer] Writer to encode to
         * @returns {$protobuf.Writer} Writer
         */
        UUID.encode = function encode(message, writer) {
            if (!writer)
                writer = $Writer.create();
            if (message.Value != null && Object.hasOwnProperty.call(message, "Value"))
                writer.uint32(/* id 1, wireType 2 =*/10).bytes(message.Value);
            return writer;
        };

        /**
         * Encodes the specified UUID message, length delimited. Does not implicitly {@link apibtc.UUID.verify|verify} messages.
         * @function encodeDelimited
         * @memberof apibtc.UUID
         * @static
         * @param {apibtc.IUUID} message UUID message or plain object to encode
         * @param {$protobuf.Writer} [writer] Writer to encode to
         * @returns {$protobuf.Writer} Writer
         */
        UUID.encodeDelimited = function encodeDelimited(message, writer) {
            return this.encode(message, writer).ldelim();
        };

        /**
         * Decodes a UUID message from the specified reader or buffer.
         * @function decode
         * @memberof apibtc.UUID
         * @static
         * @param {$protobuf.Reader|Uint8Array} reader Reader or buffer to decode from
         * @param {number} [length] Message length if known beforehand
         * @returns {apibtc.UUID} UUID
         * @throws {Error} If the payload is not a reader or valid buffer
         * @throws {$protobuf.util.ProtocolError} If required fields are missing
         */
        UUID.decode = function decode(reader, length) {
            if (!(reader instanceof $Reader))
                reader = $Reader.create(reader);
            let end = length === undefined ? reader.len : reader.pos + length, message = new $root.apibtc.UUID();
            while (reader.pos < end) {
                let tag = reader.uint32();
                switch (tag >>> 3) {
                case 1: {
                        message.Value = reader.bytes();
                        break;
                    }
                default:
                    reader.skipType(tag & 7);
                    break;
                }
            }
            return message;
        };

        /**
         * Decodes a UUID message from the specified reader or buffer, length delimited.
         * @function decodeDelimited
         * @memberof apibtc.UUID
         * @static
         * @param {$protobuf.Reader|Uint8Array} reader Reader or buffer to decode from
         * @returns {apibtc.UUID} UUID
         * @throws {Error} If the payload is not a reader or valid buffer
         * @throws {$protobuf.util.ProtocolError} If required fields are missing
         */
        UUID.decodeDelimited = function decodeDelimited(reader) {
            if (!(reader instanceof $Reader))
                reader = new $Reader(reader);
            return this.decode(reader, reader.uint32());
        };

        /**
         * Verifies a UUID message.
         * @function verify
         * @memberof apibtc.UUID
         * @static
         * @param {Object.<string,*>} message Plain object to verify
         * @returns {string|null} `null` if valid, otherwise the reason why it is not
         */
        UUID.verify = function verify(message) {
            if (typeof message !== "object" || message === null)
                return "object expected";
            if (message.Value != null && message.hasOwnProperty("Value"))
                if (!(message.Value && typeof message.Value.length === "number" || $util.isString(message.Value)))
                    return "Value: buffer expected";
            return null;
        };

        /**
         * Creates a UUID message from a plain object. Also converts values to their respective internal types.
         * @function fromObject
         * @memberof apibtc.UUID
         * @static
         * @param {Object.<string,*>} object Plain object
         * @returns {apibtc.UUID} UUID
         */
        UUID.fromObject = function fromObject(object) {
            if (object instanceof $root.apibtc.UUID)
                return object;
            let message = new $root.apibtc.UUID();
            if (object.Value != null)
                if (typeof object.Value === "string")
                    $util.base64.decode(object.Value, message.Value = $util.newBuffer($util.base64.length(object.Value)), 0);
                else if (object.Value.length >= 0)
                    message.Value = object.Value;
            return message;
        };

        /**
         * Creates a plain object from a UUID message. Also converts values to other types if specified.
         * @function toObject
         * @memberof apibtc.UUID
         * @static
         * @param {apibtc.UUID} message UUID
         * @param {$protobuf.IConversionOptions} [options] Conversion options
         * @returns {Object.<string,*>} Plain object
         */
        UUID.toObject = function toObject(message, options) {
            if (!options)
                options = {};
            let object = {};
            if (options.defaults)
                if (options.bytes === String)
                    object.Value = "";
                else {
                    object.Value = [];
                    if (options.bytes !== Array)
                        object.Value = $util.newBuffer(object.Value);
                }
            if (message.Value != null && message.hasOwnProperty("Value"))
                object.Value = options.bytes === String ? $util.base64.encode(message.Value, 0, message.Value.length) : options.bytes === Array ? Array.prototype.slice.call(message.Value) : message.Value;
            return object;
        };

        /**
         * Converts this UUID to JSON.
         * @function toJSON
         * @memberof apibtc.UUID
         * @instance
         * @returns {Object.<string,*>} JSON object
         */
        UUID.prototype.toJSON = function toJSON() {
            return this.constructor.toObject(this, $protobuf.util.toJSONOptions);
        };

        /**
         * Gets the default type url for UUID
         * @function getTypeUrl
         * @memberof apibtc.UUID
         * @static
         * @param {string} [typeUrlPrefix] your custom typeUrlPrefix(default "type.googleapis.com")
         * @returns {string} The default type url
         */
        UUID.getTypeUrl = function getTypeUrl(typeUrlPrefix) {
            if (typeUrlPrefix === undefined) {
                typeUrlPrefix = "type.googleapis.com";
            }
            return typeUrlPrefix + "/apibtc.UUID";
        };

        return UUID;
    })();

    apibtc.Timestamp = (function() {

        /**
         * Properties of a Timestamp.
         * @memberof apibtc
         * @interface ITimestamp
         * @property {number|Long|null} [Value] Timestamp Value
         */

        /**
         * Constructs a new Timestamp.
         * @memberof apibtc
         * @classdesc Represents a Timestamp.
         * @implements ITimestamp
         * @constructor
         * @param {apibtc.ITimestamp=} [properties] Properties to set
         */
        function Timestamp(properties) {
            if (properties)
                for (let keys = Object.keys(properties), i = 0; i < keys.length; ++i)
                    if (properties[keys[i]] != null)
                        this[keys[i]] = properties[keys[i]];
        }

        /**
         * Timestamp Value.
         * @member {number|Long} Value
         * @memberof apibtc.Timestamp
         * @instance
         */
        Timestamp.prototype.Value = $util.Long ? $util.Long.fromBits(0,0,false) : 0;

        /**
         * Creates a new Timestamp instance using the specified properties.
         * @function create
         * @memberof apibtc.Timestamp
         * @static
         * @param {apibtc.ITimestamp=} [properties] Properties to set
         * @returns {apibtc.Timestamp} Timestamp instance
         */
        Timestamp.create = function create(properties) {
            return new Timestamp(properties);
        };

        /**
         * Encodes the specified Timestamp message. Does not implicitly {@link apibtc.Timestamp.verify|verify} messages.
         * @function encode
         * @memberof apibtc.Timestamp
         * @static
         * @param {apibtc.ITimestamp} message Timestamp message or plain object to encode
         * @param {$protobuf.Writer} [writer] Writer to encode to
         * @returns {$protobuf.Writer} Writer
         */
        Timestamp.encode = function encode(message, writer) {
            if (!writer)
                writer = $Writer.create();
            if (message.Value != null && Object.hasOwnProperty.call(message, "Value"))
                writer.uint32(/* id 1, wireType 0 =*/8).int64(message.Value);
            return writer;
        };

        /**
         * Encodes the specified Timestamp message, length delimited. Does not implicitly {@link apibtc.Timestamp.verify|verify} messages.
         * @function encodeDelimited
         * @memberof apibtc.Timestamp
         * @static
         * @param {apibtc.ITimestamp} message Timestamp message or plain object to encode
         * @param {$protobuf.Writer} [writer] Writer to encode to
         * @returns {$protobuf.Writer} Writer
         */
        Timestamp.encodeDelimited = function encodeDelimited(message, writer) {
            return this.encode(message, writer).ldelim();
        };

        /**
         * Decodes a Timestamp message from the specified reader or buffer.
         * @function decode
         * @memberof apibtc.Timestamp
         * @static
         * @param {$protobuf.Reader|Uint8Array} reader Reader or buffer to decode from
         * @param {number} [length] Message length if known beforehand
         * @returns {apibtc.Timestamp} Timestamp
         * @throws {Error} If the payload is not a reader or valid buffer
         * @throws {$protobuf.util.ProtocolError} If required fields are missing
         */
        Timestamp.decode = function decode(reader, length) {
            if (!(reader instanceof $Reader))
                reader = $Reader.create(reader);
            let end = length === undefined ? reader.len : reader.pos + length, message = new $root.apibtc.Timestamp();
            while (reader.pos < end) {
                let tag = reader.uint32();
                switch (tag >>> 3) {
                case 1: {
                        message.Value = reader.int64();
                        break;
                    }
                default:
                    reader.skipType(tag & 7);
                    break;
                }
            }
            return message;
        };

        /**
         * Decodes a Timestamp message from the specified reader or buffer, length delimited.
         * @function decodeDelimited
         * @memberof apibtc.Timestamp
         * @static
         * @param {$protobuf.Reader|Uint8Array} reader Reader or buffer to decode from
         * @returns {apibtc.Timestamp} Timestamp
         * @throws {Error} If the payload is not a reader or valid buffer
         * @throws {$protobuf.util.ProtocolError} If required fields are missing
         */
        Timestamp.decodeDelimited = function decodeDelimited(reader) {
            if (!(reader instanceof $Reader))
                reader = new $Reader(reader);
            return this.decode(reader, reader.uint32());
        };

        /**
         * Verifies a Timestamp message.
         * @function verify
         * @memberof apibtc.Timestamp
         * @static
         * @param {Object.<string,*>} message Plain object to verify
         * @returns {string|null} `null` if valid, otherwise the reason why it is not
         */
        Timestamp.verify = function verify(message) {
            if (typeof message !== "object" || message === null)
                return "object expected";
            if (message.Value != null && message.hasOwnProperty("Value"))
                if (!$util.isInteger(message.Value) && !(message.Value && $util.isInteger(message.Value.low) && $util.isInteger(message.Value.high)))
                    return "Value: integer|Long expected";
            return null;
        };

        /**
         * Creates a Timestamp message from a plain object. Also converts values to their respective internal types.
         * @function fromObject
         * @memberof apibtc.Timestamp
         * @static
         * @param {Object.<string,*>} object Plain object
         * @returns {apibtc.Timestamp} Timestamp
         */
        Timestamp.fromObject = function fromObject(object) {
            if (object instanceof $root.apibtc.Timestamp)
                return object;
            let message = new $root.apibtc.Timestamp();
            if (object.Value != null)
                if ($util.Long)
                    (message.Value = $util.Long.fromValue(object.Value)).unsigned = false;
                else if (typeof object.Value === "string")
                    message.Value = parseInt(object.Value, 10);
                else if (typeof object.Value === "number")
                    message.Value = object.Value;
                else if (typeof object.Value === "object")
                    message.Value = new $util.LongBits(object.Value.low >>> 0, object.Value.high >>> 0).toNumber();
            return message;
        };

        /**
         * Creates a plain object from a Timestamp message. Also converts values to other types if specified.
         * @function toObject
         * @memberof apibtc.Timestamp
         * @static
         * @param {apibtc.Timestamp} message Timestamp
         * @param {$protobuf.IConversionOptions} [options] Conversion options
         * @returns {Object.<string,*>} Plain object
         */
        Timestamp.toObject = function toObject(message, options) {
            if (!options)
                options = {};
            let object = {};
            if (options.defaults)
                if ($util.Long) {
                    let long = new $util.Long(0, 0, false);
                    object.Value = options.longs === String ? long.toString() : options.longs === Number ? long.toNumber() : long;
                } else
                    object.Value = options.longs === String ? "0" : 0;
            if (message.Value != null && message.hasOwnProperty("Value"))
                if (typeof message.Value === "number")
                    object.Value = options.longs === String ? String(message.Value) : message.Value;
                else
                    object.Value = options.longs === String ? $util.Long.prototype.toString.call(message.Value) : options.longs === Number ? new $util.LongBits(message.Value.low >>> 0, message.Value.high >>> 0).toNumber() : message.Value;
            return object;
        };

        /**
         * Converts this Timestamp to JSON.
         * @function toJSON
         * @memberof apibtc.Timestamp
         * @instance
         * @returns {Object.<string,*>} JSON object
         */
        Timestamp.prototype.toJSON = function toJSON() {
            return this.constructor.toObject(this, $protobuf.util.toJSONOptions);
        };

        /**
         * Gets the default type url for Timestamp
         * @function getTypeUrl
         * @memberof apibtc.Timestamp
         * @static
         * @param {string} [typeUrlPrefix] your custom typeUrlPrefix(default "type.googleapis.com")
         * @returns {string} The default type url
         */
        Timestamp.getTypeUrl = function getTypeUrl(typeUrlPrefix) {
            if (typeUrlPrefix === undefined) {
                typeUrlPrefix = "type.googleapis.com";
            }
            return typeUrlPrefix + "/apibtc.Timestamp";
        };

        return Timestamp;
    })();

    apibtc.Signature = (function() {

        /**
         * Properties of a Signature.
         * @memberof apibtc
         * @interface ISignature
         * @property {Uint8Array|null} [Value] Signature Value
         */

        /**
         * Constructs a new Signature.
         * @memberof apibtc
         * @classdesc Represents a Signature.
         * @implements ISignature
         * @constructor
         * @param {apibtc.ISignature=} [properties] Properties to set
         */
        function Signature(properties) {
            if (properties)
                for (let keys = Object.keys(properties), i = 0; i < keys.length; ++i)
                    if (properties[keys[i]] != null)
                        this[keys[i]] = properties[keys[i]];
        }

        /**
         * Signature Value.
         * @member {Uint8Array} Value
         * @memberof apibtc.Signature
         * @instance
         */
        Signature.prototype.Value = $util.newBuffer([]);

        /**
         * Creates a new Signature instance using the specified properties.
         * @function create
         * @memberof apibtc.Signature
         * @static
         * @param {apibtc.ISignature=} [properties] Properties to set
         * @returns {apibtc.Signature} Signature instance
         */
        Signature.create = function create(properties) {
            return new Signature(properties);
        };

        /**
         * Encodes the specified Signature message. Does not implicitly {@link apibtc.Signature.verify|verify} messages.
         * @function encode
         * @memberof apibtc.Signature
         * @static
         * @param {apibtc.ISignature} message Signature message or plain object to encode
         * @param {$protobuf.Writer} [writer] Writer to encode to
         * @returns {$protobuf.Writer} Writer
         */
        Signature.encode = function encode(message, writer) {
            if (!writer)
                writer = $Writer.create();
            if (message.Value != null && Object.hasOwnProperty.call(message, "Value"))
                writer.uint32(/* id 1, wireType 2 =*/10).bytes(message.Value);
            return writer;
        };

        /**
         * Encodes the specified Signature message, length delimited. Does not implicitly {@link apibtc.Signature.verify|verify} messages.
         * @function encodeDelimited
         * @memberof apibtc.Signature
         * @static
         * @param {apibtc.ISignature} message Signature message or plain object to encode
         * @param {$protobuf.Writer} [writer] Writer to encode to
         * @returns {$protobuf.Writer} Writer
         */
        Signature.encodeDelimited = function encodeDelimited(message, writer) {
            return this.encode(message, writer).ldelim();
        };

        /**
         * Decodes a Signature message from the specified reader or buffer.
         * @function decode
         * @memberof apibtc.Signature
         * @static
         * @param {$protobuf.Reader|Uint8Array} reader Reader or buffer to decode from
         * @param {number} [length] Message length if known beforehand
         * @returns {apibtc.Signature} Signature
         * @throws {Error} If the payload is not a reader or valid buffer
         * @throws {$protobuf.util.ProtocolError} If required fields are missing
         */
        Signature.decode = function decode(reader, length) {
            if (!(reader instanceof $Reader))
                reader = $Reader.create(reader);
            let end = length === undefined ? reader.len : reader.pos + length, message = new $root.apibtc.Signature();
            while (reader.pos < end) {
                let tag = reader.uint32();
                switch (tag >>> 3) {
                case 1: {
                        message.Value = reader.bytes();
                        break;
                    }
                default:
                    reader.skipType(tag & 7);
                    break;
                }
            }
            return message;
        };

        /**
         * Decodes a Signature message from the specified reader or buffer, length delimited.
         * @function decodeDelimited
         * @memberof apibtc.Signature
         * @static
         * @param {$protobuf.Reader|Uint8Array} reader Reader or buffer to decode from
         * @returns {apibtc.Signature} Signature
         * @throws {Error} If the payload is not a reader or valid buffer
         * @throws {$protobuf.util.ProtocolError} If required fields are missing
         */
        Signature.decodeDelimited = function decodeDelimited(reader) {
            if (!(reader instanceof $Reader))
                reader = new $Reader(reader);
            return this.decode(reader, reader.uint32());
        };

        /**
         * Verifies a Signature message.
         * @function verify
         * @memberof apibtc.Signature
         * @static
         * @param {Object.<string,*>} message Plain object to verify
         * @returns {string|null} `null` if valid, otherwise the reason why it is not
         */
        Signature.verify = function verify(message) {
            if (typeof message !== "object" || message === null)
                return "object expected";
            if (message.Value != null && message.hasOwnProperty("Value"))
                if (!(message.Value && typeof message.Value.length === "number" || $util.isString(message.Value)))
                    return "Value: buffer expected";
            return null;
        };

        /**
         * Creates a Signature message from a plain object. Also converts values to their respective internal types.
         * @function fromObject
         * @memberof apibtc.Signature
         * @static
         * @param {Object.<string,*>} object Plain object
         * @returns {apibtc.Signature} Signature
         */
        Signature.fromObject = function fromObject(object) {
            if (object instanceof $root.apibtc.Signature)
                return object;
            let message = new $root.apibtc.Signature();
            if (object.Value != null)
                if (typeof object.Value === "string")
                    $util.base64.decode(object.Value, message.Value = $util.newBuffer($util.base64.length(object.Value)), 0);
                else if (object.Value.length >= 0)
                    message.Value = object.Value;
            return message;
        };

        /**
         * Creates a plain object from a Signature message. Also converts values to other types if specified.
         * @function toObject
         * @memberof apibtc.Signature
         * @static
         * @param {apibtc.Signature} message Signature
         * @param {$protobuf.IConversionOptions} [options] Conversion options
         * @returns {Object.<string,*>} Plain object
         */
        Signature.toObject = function toObject(message, options) {
            if (!options)
                options = {};
            let object = {};
            if (options.defaults)
                if (options.bytes === String)
                    object.Value = "";
                else {
                    object.Value = [];
                    if (options.bytes !== Array)
                        object.Value = $util.newBuffer(object.Value);
                }
            if (message.Value != null && message.hasOwnProperty("Value"))
                object.Value = options.bytes === String ? $util.base64.encode(message.Value, 0, message.Value.length) : options.bytes === Array ? Array.prototype.slice.call(message.Value) : message.Value;
            return object;
        };

        /**
         * Converts this Signature to JSON.
         * @function toJSON
         * @memberof apibtc.Signature
         * @instance
         * @returns {Object.<string,*>} JSON object
         */
        Signature.prototype.toJSON = function toJSON() {
            return this.constructor.toObject(this, $protobuf.util.toJSONOptions);
        };

        /**
         * Gets the default type url for Signature
         * @function getTypeUrl
         * @memberof apibtc.Signature
         * @static
         * @param {string} [typeUrlPrefix] your custom typeUrlPrefix(default "type.googleapis.com")
         * @returns {string} The default type url
         */
        Signature.getTypeUrl = function getTypeUrl(typeUrlPrefix) {
            if (typeUrlPrefix === undefined) {
                typeUrlPrefix = "type.googleapis.com";
            }
            return typeUrlPrefix + "/apibtc.Signature";
        };

        return Signature;
    })();

    apibtc.PublicKey = (function() {

        /**
         * Properties of a PublicKey.
         * @memberof apibtc
         * @interface IPublicKey
         * @property {Uint8Array|null} [Value] PublicKey Value
         */

        /**
         * Constructs a new PublicKey.
         * @memberof apibtc
         * @classdesc Represents a PublicKey.
         * @implements IPublicKey
         * @constructor
         * @param {apibtc.IPublicKey=} [properties] Properties to set
         */
        function PublicKey(properties) {
            if (properties)
                for (let keys = Object.keys(properties), i = 0; i < keys.length; ++i)
                    if (properties[keys[i]] != null)
                        this[keys[i]] = properties[keys[i]];
        }

        /**
         * PublicKey Value.
         * @member {Uint8Array} Value
         * @memberof apibtc.PublicKey
         * @instance
         */
        PublicKey.prototype.Value = $util.newBuffer([]);

        /**
         * Creates a new PublicKey instance using the specified properties.
         * @function create
         * @memberof apibtc.PublicKey
         * @static
         * @param {apibtc.IPublicKey=} [properties] Properties to set
         * @returns {apibtc.PublicKey} PublicKey instance
         */
        PublicKey.create = function create(properties) {
            return new PublicKey(properties);
        };

        /**
         * Encodes the specified PublicKey message. Does not implicitly {@link apibtc.PublicKey.verify|verify} messages.
         * @function encode
         * @memberof apibtc.PublicKey
         * @static
         * @param {apibtc.IPublicKey} message PublicKey message or plain object to encode
         * @param {$protobuf.Writer} [writer] Writer to encode to
         * @returns {$protobuf.Writer} Writer
         */
        PublicKey.encode = function encode(message, writer) {
            if (!writer)
                writer = $Writer.create();
            if (message.Value != null && Object.hasOwnProperty.call(message, "Value"))
                writer.uint32(/* id 1, wireType 2 =*/10).bytes(message.Value);
            return writer;
        };

        /**
         * Encodes the specified PublicKey message, length delimited. Does not implicitly {@link apibtc.PublicKey.verify|verify} messages.
         * @function encodeDelimited
         * @memberof apibtc.PublicKey
         * @static
         * @param {apibtc.IPublicKey} message PublicKey message or plain object to encode
         * @param {$protobuf.Writer} [writer] Writer to encode to
         * @returns {$protobuf.Writer} Writer
         */
        PublicKey.encodeDelimited = function encodeDelimited(message, writer) {
            return this.encode(message, writer).ldelim();
        };

        /**
         * Decodes a PublicKey message from the specified reader or buffer.
         * @function decode
         * @memberof apibtc.PublicKey
         * @static
         * @param {$protobuf.Reader|Uint8Array} reader Reader or buffer to decode from
         * @param {number} [length] Message length if known beforehand
         * @returns {apibtc.PublicKey} PublicKey
         * @throws {Error} If the payload is not a reader or valid buffer
         * @throws {$protobuf.util.ProtocolError} If required fields are missing
         */
        PublicKey.decode = function decode(reader, length) {
            if (!(reader instanceof $Reader))
                reader = $Reader.create(reader);
            let end = length === undefined ? reader.len : reader.pos + length, message = new $root.apibtc.PublicKey();
            while (reader.pos < end) {
                let tag = reader.uint32();
                switch (tag >>> 3) {
                case 1: {
                        message.Value = reader.bytes();
                        break;
                    }
                default:
                    reader.skipType(tag & 7);
                    break;
                }
            }
            return message;
        };

        /**
         * Decodes a PublicKey message from the specified reader or buffer, length delimited.
         * @function decodeDelimited
         * @memberof apibtc.PublicKey
         * @static
         * @param {$protobuf.Reader|Uint8Array} reader Reader or buffer to decode from
         * @returns {apibtc.PublicKey} PublicKey
         * @throws {Error} If the payload is not a reader or valid buffer
         * @throws {$protobuf.util.ProtocolError} If required fields are missing
         */
        PublicKey.decodeDelimited = function decodeDelimited(reader) {
            if (!(reader instanceof $Reader))
                reader = new $Reader(reader);
            return this.decode(reader, reader.uint32());
        };

        /**
         * Verifies a PublicKey message.
         * @function verify
         * @memberof apibtc.PublicKey
         * @static
         * @param {Object.<string,*>} message Plain object to verify
         * @returns {string|null} `null` if valid, otherwise the reason why it is not
         */
        PublicKey.verify = function verify(message) {
            if (typeof message !== "object" || message === null)
                return "object expected";
            if (message.Value != null && message.hasOwnProperty("Value"))
                if (!(message.Value && typeof message.Value.length === "number" || $util.isString(message.Value)))
                    return "Value: buffer expected";
            return null;
        };

        /**
         * Creates a PublicKey message from a plain object. Also converts values to their respective internal types.
         * @function fromObject
         * @memberof apibtc.PublicKey
         * @static
         * @param {Object.<string,*>} object Plain object
         * @returns {apibtc.PublicKey} PublicKey
         */
        PublicKey.fromObject = function fromObject(object) {
            if (object instanceof $root.apibtc.PublicKey)
                return object;
            let message = new $root.apibtc.PublicKey();
            if (object.Value != null)
                if (typeof object.Value === "string")
                    $util.base64.decode(object.Value, message.Value = $util.newBuffer($util.base64.length(object.Value)), 0);
                else if (object.Value.length >= 0)
                    message.Value = object.Value;
            return message;
        };

        /**
         * Creates a plain object from a PublicKey message. Also converts values to other types if specified.
         * @function toObject
         * @memberof apibtc.PublicKey
         * @static
         * @param {apibtc.PublicKey} message PublicKey
         * @param {$protobuf.IConversionOptions} [options] Conversion options
         * @returns {Object.<string,*>} Plain object
         */
        PublicKey.toObject = function toObject(message, options) {
            if (!options)
                options = {};
            let object = {};
            if (options.defaults)
                if (options.bytes === String)
                    object.Value = "";
                else {
                    object.Value = [];
                    if (options.bytes !== Array)
                        object.Value = $util.newBuffer(object.Value);
                }
            if (message.Value != null && message.hasOwnProperty("Value"))
                object.Value = options.bytes === String ? $util.base64.encode(message.Value, 0, message.Value.length) : options.bytes === Array ? Array.prototype.slice.call(message.Value) : message.Value;
            return object;
        };

        /**
         * Converts this PublicKey to JSON.
         * @function toJSON
         * @memberof apibtc.PublicKey
         * @instance
         * @returns {Object.<string,*>} JSON object
         */
        PublicKey.prototype.toJSON = function toJSON() {
            return this.constructor.toObject(this, $protobuf.util.toJSONOptions);
        };

        /**
         * Gets the default type url for PublicKey
         * @function getTypeUrl
         * @memberof apibtc.PublicKey
         * @static
         * @param {string} [typeUrlPrefix] your custom typeUrlPrefix(default "type.googleapis.com")
         * @returns {string} The default type url
         */
        PublicKey.getTypeUrl = function getTypeUrl(typeUrlPrefix) {
            if (typeUrlPrefix === undefined) {
                typeUrlPrefix = "type.googleapis.com";
            }
            return typeUrlPrefix + "/apibtc.PublicKey";
        };

        return PublicKey;
    })();

    apibtc.AuthTokenHeader = (function() {

        /**
         * Properties of an AuthTokenHeader.
         * @memberof apibtc
         * @interface IAuthTokenHeader
         * @property {apibtc.IUUID|null} [TokenId] AuthTokenHeader TokenId
         * @property {apibtc.IPublicKey|null} [PublicKey] AuthTokenHeader PublicKey
         * @property {apibtc.ITimestamp|null} [Timestamp] AuthTokenHeader Timestamp
         */

        /**
         * Constructs a new AuthTokenHeader.
         * @memberof apibtc
         * @classdesc Represents an AuthTokenHeader.
         * @implements IAuthTokenHeader
         * @constructor
         * @param {apibtc.IAuthTokenHeader=} [properties] Properties to set
         */
        function AuthTokenHeader(properties) {
            if (properties)
                for (let keys = Object.keys(properties), i = 0; i < keys.length; ++i)
                    if (properties[keys[i]] != null)
                        this[keys[i]] = properties[keys[i]];
        }

        /**
         * AuthTokenHeader TokenId.
         * @member {apibtc.IUUID|null|undefined} TokenId
         * @memberof apibtc.AuthTokenHeader
         * @instance
         */
        AuthTokenHeader.prototype.TokenId = null;

        /**
         * AuthTokenHeader PublicKey.
         * @member {apibtc.IPublicKey|null|undefined} PublicKey
         * @memberof apibtc.AuthTokenHeader
         * @instance
         */
        AuthTokenHeader.prototype.PublicKey = null;

        /**
         * AuthTokenHeader Timestamp.
         * @member {apibtc.ITimestamp|null|undefined} Timestamp
         * @memberof apibtc.AuthTokenHeader
         * @instance
         */
        AuthTokenHeader.prototype.Timestamp = null;

        /**
         * Creates a new AuthTokenHeader instance using the specified properties.
         * @function create
         * @memberof apibtc.AuthTokenHeader
         * @static
         * @param {apibtc.IAuthTokenHeader=} [properties] Properties to set
         * @returns {apibtc.AuthTokenHeader} AuthTokenHeader instance
         */
        AuthTokenHeader.create = function create(properties) {
            return new AuthTokenHeader(properties);
        };

        /**
         * Encodes the specified AuthTokenHeader message. Does not implicitly {@link apibtc.AuthTokenHeader.verify|verify} messages.
         * @function encode
         * @memberof apibtc.AuthTokenHeader
         * @static
         * @param {apibtc.IAuthTokenHeader} message AuthTokenHeader message or plain object to encode
         * @param {$protobuf.Writer} [writer] Writer to encode to
         * @returns {$protobuf.Writer} Writer
         */
        AuthTokenHeader.encode = function encode(message, writer) {
            if (!writer)
                writer = $Writer.create();
            if (message.TokenId != null && Object.hasOwnProperty.call(message, "TokenId"))
                $root.apibtc.UUID.encode(message.TokenId, writer.uint32(/* id 1, wireType 2 =*/10).fork()).ldelim();
            if (message.PublicKey != null && Object.hasOwnProperty.call(message, "PublicKey"))
                $root.apibtc.PublicKey.encode(message.PublicKey, writer.uint32(/* id 2, wireType 2 =*/18).fork()).ldelim();
            if (message.Timestamp != null && Object.hasOwnProperty.call(message, "Timestamp"))
                $root.apibtc.Timestamp.encode(message.Timestamp, writer.uint32(/* id 3, wireType 2 =*/26).fork()).ldelim();
            return writer;
        };

        /**
         * Encodes the specified AuthTokenHeader message, length delimited. Does not implicitly {@link apibtc.AuthTokenHeader.verify|verify} messages.
         * @function encodeDelimited
         * @memberof apibtc.AuthTokenHeader
         * @static
         * @param {apibtc.IAuthTokenHeader} message AuthTokenHeader message or plain object to encode
         * @param {$protobuf.Writer} [writer] Writer to encode to
         * @returns {$protobuf.Writer} Writer
         */
        AuthTokenHeader.encodeDelimited = function encodeDelimited(message, writer) {
            return this.encode(message, writer).ldelim();
        };

        /**
         * Decodes an AuthTokenHeader message from the specified reader or buffer.
         * @function decode
         * @memberof apibtc.AuthTokenHeader
         * @static
         * @param {$protobuf.Reader|Uint8Array} reader Reader or buffer to decode from
         * @param {number} [length] Message length if known beforehand
         * @returns {apibtc.AuthTokenHeader} AuthTokenHeader
         * @throws {Error} If the payload is not a reader or valid buffer
         * @throws {$protobuf.util.ProtocolError} If required fields are missing
         */
        AuthTokenHeader.decode = function decode(reader, length) {
            if (!(reader instanceof $Reader))
                reader = $Reader.create(reader);
            let end = length === undefined ? reader.len : reader.pos + length, message = new $root.apibtc.AuthTokenHeader();
            while (reader.pos < end) {
                let tag = reader.uint32();
                switch (tag >>> 3) {
                case 1: {
                        message.TokenId = $root.apibtc.UUID.decode(reader, reader.uint32());
                        break;
                    }
                case 2: {
                        message.PublicKey = $root.apibtc.PublicKey.decode(reader, reader.uint32());
                        break;
                    }
                case 3: {
                        message.Timestamp = $root.apibtc.Timestamp.decode(reader, reader.uint32());
                        break;
                    }
                default:
                    reader.skipType(tag & 7);
                    break;
                }
            }
            return message;
        };

        /**
         * Decodes an AuthTokenHeader message from the specified reader or buffer, length delimited.
         * @function decodeDelimited
         * @memberof apibtc.AuthTokenHeader
         * @static
         * @param {$protobuf.Reader|Uint8Array} reader Reader or buffer to decode from
         * @returns {apibtc.AuthTokenHeader} AuthTokenHeader
         * @throws {Error} If the payload is not a reader or valid buffer
         * @throws {$protobuf.util.ProtocolError} If required fields are missing
         */
        AuthTokenHeader.decodeDelimited = function decodeDelimited(reader) {
            if (!(reader instanceof $Reader))
                reader = new $Reader(reader);
            return this.decode(reader, reader.uint32());
        };

        /**
         * Verifies an AuthTokenHeader message.
         * @function verify
         * @memberof apibtc.AuthTokenHeader
         * @static
         * @param {Object.<string,*>} message Plain object to verify
         * @returns {string|null} `null` if valid, otherwise the reason why it is not
         */
        AuthTokenHeader.verify = function verify(message) {
            if (typeof message !== "object" || message === null)
                return "object expected";
            if (message.TokenId != null && message.hasOwnProperty("TokenId")) {
                let error = $root.apibtc.UUID.verify(message.TokenId);
                if (error)
                    return "TokenId." + error;
            }
            if (message.PublicKey != null && message.hasOwnProperty("PublicKey")) {
                let error = $root.apibtc.PublicKey.verify(message.PublicKey);
                if (error)
                    return "PublicKey." + error;
            }
            if (message.Timestamp != null && message.hasOwnProperty("Timestamp")) {
                let error = $root.apibtc.Timestamp.verify(message.Timestamp);
                if (error)
                    return "Timestamp." + error;
            }
            return null;
        };

        /**
         * Creates an AuthTokenHeader message from a plain object. Also converts values to their respective internal types.
         * @function fromObject
         * @memberof apibtc.AuthTokenHeader
         * @static
         * @param {Object.<string,*>} object Plain object
         * @returns {apibtc.AuthTokenHeader} AuthTokenHeader
         */
        AuthTokenHeader.fromObject = function fromObject(object) {
            if (object instanceof $root.apibtc.AuthTokenHeader)
                return object;
            let message = new $root.apibtc.AuthTokenHeader();
            if (object.TokenId != null) {
                if (typeof object.TokenId !== "object")
                    throw TypeError(".apibtc.AuthTokenHeader.TokenId: object expected");
                message.TokenId = $root.apibtc.UUID.fromObject(object.TokenId);
            }
            if (object.PublicKey != null) {
                if (typeof object.PublicKey !== "object")
                    throw TypeError(".apibtc.AuthTokenHeader.PublicKey: object expected");
                message.PublicKey = $root.apibtc.PublicKey.fromObject(object.PublicKey);
            }
            if (object.Timestamp != null) {
                if (typeof object.Timestamp !== "object")
                    throw TypeError(".apibtc.AuthTokenHeader.Timestamp: object expected");
                message.Timestamp = $root.apibtc.Timestamp.fromObject(object.Timestamp);
            }
            return message;
        };

        /**
         * Creates a plain object from an AuthTokenHeader message. Also converts values to other types if specified.
         * @function toObject
         * @memberof apibtc.AuthTokenHeader
         * @static
         * @param {apibtc.AuthTokenHeader} message AuthTokenHeader
         * @param {$protobuf.IConversionOptions} [options] Conversion options
         * @returns {Object.<string,*>} Plain object
         */
        AuthTokenHeader.toObject = function toObject(message, options) {
            if (!options)
                options = {};
            let object = {};
            if (options.defaults) {
                object.TokenId = null;
                object.PublicKey = null;
                object.Timestamp = null;
            }
            if (message.TokenId != null && message.hasOwnProperty("TokenId"))
                object.TokenId = $root.apibtc.UUID.toObject(message.TokenId, options);
            if (message.PublicKey != null && message.hasOwnProperty("PublicKey"))
                object.PublicKey = $root.apibtc.PublicKey.toObject(message.PublicKey, options);
            if (message.Timestamp != null && message.hasOwnProperty("Timestamp"))
                object.Timestamp = $root.apibtc.Timestamp.toObject(message.Timestamp, options);
            return object;
        };

        /**
         * Converts this AuthTokenHeader to JSON.
         * @function toJSON
         * @memberof apibtc.AuthTokenHeader
         * @instance
         * @returns {Object.<string,*>} JSON object
         */
        AuthTokenHeader.prototype.toJSON = function toJSON() {
            return this.constructor.toObject(this, $protobuf.util.toJSONOptions);
        };

        /**
         * Gets the default type url for AuthTokenHeader
         * @function getTypeUrl
         * @memberof apibtc.AuthTokenHeader
         * @static
         * @param {string} [typeUrlPrefix] your custom typeUrlPrefix(default "type.googleapis.com")
         * @returns {string} The default type url
         */
        AuthTokenHeader.getTypeUrl = function getTypeUrl(typeUrlPrefix) {
            if (typeUrlPrefix === undefined) {
                typeUrlPrefix = "type.googleapis.com";
            }
            return typeUrlPrefix + "/apibtc.AuthTokenHeader";
        };

        return AuthTokenHeader;
    })();

    apibtc.AuthToken = (function() {

        /**
         * Properties of an AuthToken.
         * @memberof apibtc
         * @interface IAuthToken
         * @property {apibtc.IAuthTokenHeader|null} [Header] AuthToken Header
         * @property {apibtc.ISignature|null} [Signature] AuthToken Signature
         */

        /**
         * Constructs a new AuthToken.
         * @memberof apibtc
         * @classdesc Represents an AuthToken.
         * @implements IAuthToken
         * @constructor
         * @param {apibtc.IAuthToken=} [properties] Properties to set
         */
        function AuthToken(properties) {
            if (properties)
                for (let keys = Object.keys(properties), i = 0; i < keys.length; ++i)
                    if (properties[keys[i]] != null)
                        this[keys[i]] = properties[keys[i]];
        }

        /**
         * AuthToken Header.
         * @member {apibtc.IAuthTokenHeader|null|undefined} Header
         * @memberof apibtc.AuthToken
         * @instance
         */
        AuthToken.prototype.Header = null;

        /**
         * AuthToken Signature.
         * @member {apibtc.ISignature|null|undefined} Signature
         * @memberof apibtc.AuthToken
         * @instance
         */
        AuthToken.prototype.Signature = null;

        /**
         * Creates a new AuthToken instance using the specified properties.
         * @function create
         * @memberof apibtc.AuthToken
         * @static
         * @param {apibtc.IAuthToken=} [properties] Properties to set
         * @returns {apibtc.AuthToken} AuthToken instance
         */
        AuthToken.create = function create(properties) {
            return new AuthToken(properties);
        };

        /**
         * Encodes the specified AuthToken message. Does not implicitly {@link apibtc.AuthToken.verify|verify} messages.
         * @function encode
         * @memberof apibtc.AuthToken
         * @static
         * @param {apibtc.IAuthToken} message AuthToken message or plain object to encode
         * @param {$protobuf.Writer} [writer] Writer to encode to
         * @returns {$protobuf.Writer} Writer
         */
        AuthToken.encode = function encode(message, writer) {
            if (!writer)
                writer = $Writer.create();
            if (message.Header != null && Object.hasOwnProperty.call(message, "Header"))
                $root.apibtc.AuthTokenHeader.encode(message.Header, writer.uint32(/* id 1, wireType 2 =*/10).fork()).ldelim();
            if (message.Signature != null && Object.hasOwnProperty.call(message, "Signature"))
                $root.apibtc.Signature.encode(message.Signature, writer.uint32(/* id 2, wireType 2 =*/18).fork()).ldelim();
            return writer;
        };

        /**
         * Encodes the specified AuthToken message, length delimited. Does not implicitly {@link apibtc.AuthToken.verify|verify} messages.
         * @function encodeDelimited
         * @memberof apibtc.AuthToken
         * @static
         * @param {apibtc.IAuthToken} message AuthToken message or plain object to encode
         * @param {$protobuf.Writer} [writer] Writer to encode to
         * @returns {$protobuf.Writer} Writer
         */
        AuthToken.encodeDelimited = function encodeDelimited(message, writer) {
            return this.encode(message, writer).ldelim();
        };

        /**
         * Decodes an AuthToken message from the specified reader or buffer.
         * @function decode
         * @memberof apibtc.AuthToken
         * @static
         * @param {$protobuf.Reader|Uint8Array} reader Reader or buffer to decode from
         * @param {number} [length] Message length if known beforehand
         * @returns {apibtc.AuthToken} AuthToken
         * @throws {Error} If the payload is not a reader or valid buffer
         * @throws {$protobuf.util.ProtocolError} If required fields are missing
         */
        AuthToken.decode = function decode(reader, length) {
            if (!(reader instanceof $Reader))
                reader = $Reader.create(reader);
            let end = length === undefined ? reader.len : reader.pos + length, message = new $root.apibtc.AuthToken();
            while (reader.pos < end) {
                let tag = reader.uint32();
                switch (tag >>> 3) {
                case 1: {
                        message.Header = $root.apibtc.AuthTokenHeader.decode(reader, reader.uint32());
                        break;
                    }
                case 2: {
                        message.Signature = $root.apibtc.Signature.decode(reader, reader.uint32());
                        break;
                    }
                default:
                    reader.skipType(tag & 7);
                    break;
                }
            }
            return message;
        };

        /**
         * Decodes an AuthToken message from the specified reader or buffer, length delimited.
         * @function decodeDelimited
         * @memberof apibtc.AuthToken
         * @static
         * @param {$protobuf.Reader|Uint8Array} reader Reader or buffer to decode from
         * @returns {apibtc.AuthToken} AuthToken
         * @throws {Error} If the payload is not a reader or valid buffer
         * @throws {$protobuf.util.ProtocolError} If required fields are missing
         */
        AuthToken.decodeDelimited = function decodeDelimited(reader) {
            if (!(reader instanceof $Reader))
                reader = new $Reader(reader);
            return this.decode(reader, reader.uint32());
        };

        /**
         * Verifies an AuthToken message.
         * @function verify
         * @memberof apibtc.AuthToken
         * @static
         * @param {Object.<string,*>} message Plain object to verify
         * @returns {string|null} `null` if valid, otherwise the reason why it is not
         */
        AuthToken.verify = function verify(message) {
            if (typeof message !== "object" || message === null)
                return "object expected";
            if (message.Header != null && message.hasOwnProperty("Header")) {
                let error = $root.apibtc.AuthTokenHeader.verify(message.Header);
                if (error)
                    return "Header." + error;
            }
            if (message.Signature != null && message.hasOwnProperty("Signature")) {
                let error = $root.apibtc.Signature.verify(message.Signature);
                if (error)
                    return "Signature." + error;
            }
            return null;
        };

        /**
         * Creates an AuthToken message from a plain object. Also converts values to their respective internal types.
         * @function fromObject
         * @memberof apibtc.AuthToken
         * @static
         * @param {Object.<string,*>} object Plain object
         * @returns {apibtc.AuthToken} AuthToken
         */
        AuthToken.fromObject = function fromObject(object) {
            if (object instanceof $root.apibtc.AuthToken)
                return object;
            let message = new $root.apibtc.AuthToken();
            if (object.Header != null) {
                if (typeof object.Header !== "object")
                    throw TypeError(".apibtc.AuthToken.Header: object expected");
                message.Header = $root.apibtc.AuthTokenHeader.fromObject(object.Header);
            }
            if (object.Signature != null) {
                if (typeof object.Signature !== "object")
                    throw TypeError(".apibtc.AuthToken.Signature: object expected");
                message.Signature = $root.apibtc.Signature.fromObject(object.Signature);
            }
            return message;
        };

        /**
         * Creates a plain object from an AuthToken message. Also converts values to other types if specified.
         * @function toObject
         * @memberof apibtc.AuthToken
         * @static
         * @param {apibtc.AuthToken} message AuthToken
         * @param {$protobuf.IConversionOptions} [options] Conversion options
         * @returns {Object.<string,*>} Plain object
         */
        AuthToken.toObject = function toObject(message, options) {
            if (!options)
                options = {};
            let object = {};
            if (options.defaults) {
                object.Header = null;
                object.Signature = null;
            }
            if (message.Header != null && message.hasOwnProperty("Header"))
                object.Header = $root.apibtc.AuthTokenHeader.toObject(message.Header, options);
            if (message.Signature != null && message.hasOwnProperty("Signature"))
                object.Signature = $root.apibtc.Signature.toObject(message.Signature, options);
            return object;
        };

        /**
         * Converts this AuthToken to JSON.
         * @function toJSON
         * @memberof apibtc.AuthToken
         * @instance
         * @returns {Object.<string,*>} JSON object
         */
        AuthToken.prototype.toJSON = function toJSON() {
            return this.constructor.toObject(this, $protobuf.util.toJSONOptions);
        };

        /**
         * Gets the default type url for AuthToken
         * @function getTypeUrl
         * @memberof apibtc.AuthToken
         * @static
         * @param {string} [typeUrlPrefix] your custom typeUrlPrefix(default "type.googleapis.com")
         * @returns {string} The default type url
         */
        AuthToken.getTypeUrl = function getTypeUrl(typeUrlPrefix) {
            if (typeUrlPrefix === undefined) {
                typeUrlPrefix = "type.googleapis.com";
            }
            return typeUrlPrefix + "/apibtc.AuthToken";
        };

        return AuthToken;
    })();

    return apibtc;
})();

export { $root as default };
