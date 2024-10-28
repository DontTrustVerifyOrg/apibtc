/*eslint-disable block-scoped-var, id-length, no-control-regex, no-magic-numbers, no-prototype-builtins, no-redeclare, no-shadow, no-var, sort-vars*/
const $protobuf = require("protobufjs/minimal");

// Common aliases
const $Reader = $protobuf.Reader, $Writer = $protobuf.Writer, $util = $protobuf.util;

// Exported root namespace
const $root = $protobuf.roots["default"] || ($protobuf.roots["default"] = {});

const frames = $root.frames = (() => {

    /**
     * Namespace frames.
     * @exports frames
     * @namespace
     */
    const frames = {};

    frames.UUID = (function() {

        /**
         * Properties of a UUID.
         * @memberof frames
         * @interface IUUID
         * @property {Uint8Array|null} [Value] UUID Value
         */

        /**
         * Constructs a new UUID.
         * @memberof frames
         * @classdesc Represents a UUID.
         * @implements IUUID
         * @constructor
         * @param {frames.IUUID=} [properties] Properties to set
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
         * @memberof frames.UUID
         * @instance
         */
        UUID.prototype.Value = $util.newBuffer([]);

        /**
         * Creates a new UUID instance using the specified properties.
         * @function create
         * @memberof frames.UUID
         * @static
         * @param {frames.IUUID=} [properties] Properties to set
         * @returns {frames.UUID} UUID instance
         */
        UUID.create = function create(properties) {
            return new UUID(properties);
        };

        /**
         * Encodes the specified UUID message. Does not implicitly {@link frames.UUID.verify|verify} messages.
         * @function encode
         * @memberof frames.UUID
         * @static
         * @param {frames.IUUID} message UUID message or plain object to encode
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
         * Encodes the specified UUID message, length delimited. Does not implicitly {@link frames.UUID.verify|verify} messages.
         * @function encodeDelimited
         * @memberof frames.UUID
         * @static
         * @param {frames.IUUID} message UUID message or plain object to encode
         * @param {$protobuf.Writer} [writer] Writer to encode to
         * @returns {$protobuf.Writer} Writer
         */
        UUID.encodeDelimited = function encodeDelimited(message, writer) {
            return this.encode(message, writer).ldelim();
        };

        /**
         * Decodes a UUID message from the specified reader or buffer.
         * @function decode
         * @memberof frames.UUID
         * @static
         * @param {$protobuf.Reader|Uint8Array} reader Reader or buffer to decode from
         * @param {number} [length] Message length if known beforehand
         * @returns {frames.UUID} UUID
         * @throws {Error} If the payload is not a reader or valid buffer
         * @throws {$protobuf.util.ProtocolError} If required fields are missing
         */
        UUID.decode = function decode(reader, length) {
            if (!(reader instanceof $Reader))
                reader = $Reader.create(reader);
            let end = length === undefined ? reader.len : reader.pos + length, message = new $root.frames.UUID();
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
         * @memberof frames.UUID
         * @static
         * @param {$protobuf.Reader|Uint8Array} reader Reader or buffer to decode from
         * @returns {frames.UUID} UUID
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
         * @memberof frames.UUID
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
         * @memberof frames.UUID
         * @static
         * @param {Object.<string,*>} object Plain object
         * @returns {frames.UUID} UUID
         */
        UUID.fromObject = function fromObject(object) {
            if (object instanceof $root.frames.UUID)
                return object;
            let message = new $root.frames.UUID();
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
         * @memberof frames.UUID
         * @static
         * @param {frames.UUID} message UUID
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
         * @memberof frames.UUID
         * @instance
         * @returns {Object.<string,*>} JSON object
         */
        UUID.prototype.toJSON = function toJSON() {
            return this.constructor.toObject(this, $protobuf.util.toJSONOptions);
        };

        /**
         * Gets the default type url for UUID
         * @function getTypeUrl
         * @memberof frames.UUID
         * @static
         * @param {string} [typeUrlPrefix] your custom typeUrlPrefix(default "type.googleapis.com")
         * @returns {string} The default type url
         */
        UUID.getTypeUrl = function getTypeUrl(typeUrlPrefix) {
            if (typeUrlPrefix === undefined) {
                typeUrlPrefix = "type.googleapis.com";
            }
            return typeUrlPrefix + "/frames.UUID";
        };

        return UUID;
    })();

    frames.Timestamp = (function() {

        /**
         * Properties of a Timestamp.
         * @memberof frames
         * @interface ITimestamp
         * @property {number|Long|null} [Value] Timestamp Value
         */

        /**
         * Constructs a new Timestamp.
         * @memberof frames
         * @classdesc Represents a Timestamp.
         * @implements ITimestamp
         * @constructor
         * @param {frames.ITimestamp=} [properties] Properties to set
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
         * @memberof frames.Timestamp
         * @instance
         */
        Timestamp.prototype.Value = $util.Long ? $util.Long.fromBits(0,0,false) : 0;

        /**
         * Creates a new Timestamp instance using the specified properties.
         * @function create
         * @memberof frames.Timestamp
         * @static
         * @param {frames.ITimestamp=} [properties] Properties to set
         * @returns {frames.Timestamp} Timestamp instance
         */
        Timestamp.create = function create(properties) {
            return new Timestamp(properties);
        };

        /**
         * Encodes the specified Timestamp message. Does not implicitly {@link frames.Timestamp.verify|verify} messages.
         * @function encode
         * @memberof frames.Timestamp
         * @static
         * @param {frames.ITimestamp} message Timestamp message or plain object to encode
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
         * Encodes the specified Timestamp message, length delimited. Does not implicitly {@link frames.Timestamp.verify|verify} messages.
         * @function encodeDelimited
         * @memberof frames.Timestamp
         * @static
         * @param {frames.ITimestamp} message Timestamp message or plain object to encode
         * @param {$protobuf.Writer} [writer] Writer to encode to
         * @returns {$protobuf.Writer} Writer
         */
        Timestamp.encodeDelimited = function encodeDelimited(message, writer) {
            return this.encode(message, writer).ldelim();
        };

        /**
         * Decodes a Timestamp message from the specified reader or buffer.
         * @function decode
         * @memberof frames.Timestamp
         * @static
         * @param {$protobuf.Reader|Uint8Array} reader Reader or buffer to decode from
         * @param {number} [length] Message length if known beforehand
         * @returns {frames.Timestamp} Timestamp
         * @throws {Error} If the payload is not a reader or valid buffer
         * @throws {$protobuf.util.ProtocolError} If required fields are missing
         */
        Timestamp.decode = function decode(reader, length) {
            if (!(reader instanceof $Reader))
                reader = $Reader.create(reader);
            let end = length === undefined ? reader.len : reader.pos + length, message = new $root.frames.Timestamp();
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
         * @memberof frames.Timestamp
         * @static
         * @param {$protobuf.Reader|Uint8Array} reader Reader or buffer to decode from
         * @returns {frames.Timestamp} Timestamp
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
         * @memberof frames.Timestamp
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
         * @memberof frames.Timestamp
         * @static
         * @param {Object.<string,*>} object Plain object
         * @returns {frames.Timestamp} Timestamp
         */
        Timestamp.fromObject = function fromObject(object) {
            if (object instanceof $root.frames.Timestamp)
                return object;
            let message = new $root.frames.Timestamp();
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
         * @memberof frames.Timestamp
         * @static
         * @param {frames.Timestamp} message Timestamp
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
         * @memberof frames.Timestamp
         * @instance
         * @returns {Object.<string,*>} JSON object
         */
        Timestamp.prototype.toJSON = function toJSON() {
            return this.constructor.toObject(this, $protobuf.util.toJSONOptions);
        };

        /**
         * Gets the default type url for Timestamp
         * @function getTypeUrl
         * @memberof frames.Timestamp
         * @static
         * @param {string} [typeUrlPrefix] your custom typeUrlPrefix(default "type.googleapis.com")
         * @returns {string} The default type url
         */
        Timestamp.getTypeUrl = function getTypeUrl(typeUrlPrefix) {
            if (typeUrlPrefix === undefined) {
                typeUrlPrefix = "type.googleapis.com";
            }
            return typeUrlPrefix + "/frames.Timestamp";
        };

        return Timestamp;
    })();

    frames.URI = (function() {

        /**
         * Properties of a URI.
         * @memberof frames
         * @interface IURI
         * @property {string|null} [Value] URI Value
         */

        /**
         * Constructs a new URI.
         * @memberof frames
         * @classdesc Represents a URI.
         * @implements IURI
         * @constructor
         * @param {frames.IURI=} [properties] Properties to set
         */
        function URI(properties) {
            if (properties)
                for (let keys = Object.keys(properties), i = 0; i < keys.length; ++i)
                    if (properties[keys[i]] != null)
                        this[keys[i]] = properties[keys[i]];
        }

        /**
         * URI Value.
         * @member {string} Value
         * @memberof frames.URI
         * @instance
         */
        URI.prototype.Value = "";

        /**
         * Creates a new URI instance using the specified properties.
         * @function create
         * @memberof frames.URI
         * @static
         * @param {frames.IURI=} [properties] Properties to set
         * @returns {frames.URI} URI instance
         */
        URI.create = function create(properties) {
            return new URI(properties);
        };

        /**
         * Encodes the specified URI message. Does not implicitly {@link frames.URI.verify|verify} messages.
         * @function encode
         * @memberof frames.URI
         * @static
         * @param {frames.IURI} message URI message or plain object to encode
         * @param {$protobuf.Writer} [writer] Writer to encode to
         * @returns {$protobuf.Writer} Writer
         */
        URI.encode = function encode(message, writer) {
            if (!writer)
                writer = $Writer.create();
            if (message.Value != null && Object.hasOwnProperty.call(message, "Value"))
                writer.uint32(/* id 1, wireType 2 =*/10).string(message.Value);
            return writer;
        };

        /**
         * Encodes the specified URI message, length delimited. Does not implicitly {@link frames.URI.verify|verify} messages.
         * @function encodeDelimited
         * @memberof frames.URI
         * @static
         * @param {frames.IURI} message URI message or plain object to encode
         * @param {$protobuf.Writer} [writer] Writer to encode to
         * @returns {$protobuf.Writer} Writer
         */
        URI.encodeDelimited = function encodeDelimited(message, writer) {
            return this.encode(message, writer).ldelim();
        };

        /**
         * Decodes a URI message from the specified reader or buffer.
         * @function decode
         * @memberof frames.URI
         * @static
         * @param {$protobuf.Reader|Uint8Array} reader Reader or buffer to decode from
         * @param {number} [length] Message length if known beforehand
         * @returns {frames.URI} URI
         * @throws {Error} If the payload is not a reader or valid buffer
         * @throws {$protobuf.util.ProtocolError} If required fields are missing
         */
        URI.decode = function decode(reader, length) {
            if (!(reader instanceof $Reader))
                reader = $Reader.create(reader);
            let end = length === undefined ? reader.len : reader.pos + length, message = new $root.frames.URI();
            while (reader.pos < end) {
                let tag = reader.uint32();
                switch (tag >>> 3) {
                case 1: {
                        message.Value = reader.string();
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
         * Decodes a URI message from the specified reader or buffer, length delimited.
         * @function decodeDelimited
         * @memberof frames.URI
         * @static
         * @param {$protobuf.Reader|Uint8Array} reader Reader or buffer to decode from
         * @returns {frames.URI} URI
         * @throws {Error} If the payload is not a reader or valid buffer
         * @throws {$protobuf.util.ProtocolError} If required fields are missing
         */
        URI.decodeDelimited = function decodeDelimited(reader) {
            if (!(reader instanceof $Reader))
                reader = new $Reader(reader);
            return this.decode(reader, reader.uint32());
        };

        /**
         * Verifies a URI message.
         * @function verify
         * @memberof frames.URI
         * @static
         * @param {Object.<string,*>} message Plain object to verify
         * @returns {string|null} `null` if valid, otherwise the reason why it is not
         */
        URI.verify = function verify(message) {
            if (typeof message !== "object" || message === null)
                return "object expected";
            if (message.Value != null && message.hasOwnProperty("Value"))
                if (!$util.isString(message.Value))
                    return "Value: string expected";
            return null;
        };

        /**
         * Creates a URI message from a plain object. Also converts values to their respective internal types.
         * @function fromObject
         * @memberof frames.URI
         * @static
         * @param {Object.<string,*>} object Plain object
         * @returns {frames.URI} URI
         */
        URI.fromObject = function fromObject(object) {
            if (object instanceof $root.frames.URI)
                return object;
            let message = new $root.frames.URI();
            if (object.Value != null)
                message.Value = String(object.Value);
            return message;
        };

        /**
         * Creates a plain object from a URI message. Also converts values to other types if specified.
         * @function toObject
         * @memberof frames.URI
         * @static
         * @param {frames.URI} message URI
         * @param {$protobuf.IConversionOptions} [options] Conversion options
         * @returns {Object.<string,*>} Plain object
         */
        URI.toObject = function toObject(message, options) {
            if (!options)
                options = {};
            let object = {};
            if (options.defaults)
                object.Value = "";
            if (message.Value != null && message.hasOwnProperty("Value"))
                object.Value = message.Value;
            return object;
        };

        /**
         * Converts this URI to JSON.
         * @function toJSON
         * @memberof frames.URI
         * @instance
         * @returns {Object.<string,*>} JSON object
         */
        URI.prototype.toJSON = function toJSON() {
            return this.constructor.toObject(this, $protobuf.util.toJSONOptions);
        };

        /**
         * Gets the default type url for URI
         * @function getTypeUrl
         * @memberof frames.URI
         * @static
         * @param {string} [typeUrlPrefix] your custom typeUrlPrefix(default "type.googleapis.com")
         * @returns {string} The default type url
         */
        URI.getTypeUrl = function getTypeUrl(typeUrlPrefix) {
            if (typeUrlPrefix === undefined) {
                typeUrlPrefix = "type.googleapis.com";
            }
            return typeUrlPrefix + "/frames.URI";
        };

        return URI;
    })();

    frames.Signature = (function() {

        /**
         * Properties of a Signature.
         * @memberof frames
         * @interface ISignature
         * @property {Uint8Array|null} [Value] Signature Value
         */

        /**
         * Constructs a new Signature.
         * @memberof frames
         * @classdesc Represents a Signature.
         * @implements ISignature
         * @constructor
         * @param {frames.ISignature=} [properties] Properties to set
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
         * @memberof frames.Signature
         * @instance
         */
        Signature.prototype.Value = $util.newBuffer([]);

        /**
         * Creates a new Signature instance using the specified properties.
         * @function create
         * @memberof frames.Signature
         * @static
         * @param {frames.ISignature=} [properties] Properties to set
         * @returns {frames.Signature} Signature instance
         */
        Signature.create = function create(properties) {
            return new Signature(properties);
        };

        /**
         * Encodes the specified Signature message. Does not implicitly {@link frames.Signature.verify|verify} messages.
         * @function encode
         * @memberof frames.Signature
         * @static
         * @param {frames.ISignature} message Signature message or plain object to encode
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
         * Encodes the specified Signature message, length delimited. Does not implicitly {@link frames.Signature.verify|verify} messages.
         * @function encodeDelimited
         * @memberof frames.Signature
         * @static
         * @param {frames.ISignature} message Signature message or plain object to encode
         * @param {$protobuf.Writer} [writer] Writer to encode to
         * @returns {$protobuf.Writer} Writer
         */
        Signature.encodeDelimited = function encodeDelimited(message, writer) {
            return this.encode(message, writer).ldelim();
        };

        /**
         * Decodes a Signature message from the specified reader or buffer.
         * @function decode
         * @memberof frames.Signature
         * @static
         * @param {$protobuf.Reader|Uint8Array} reader Reader or buffer to decode from
         * @param {number} [length] Message length if known beforehand
         * @returns {frames.Signature} Signature
         * @throws {Error} If the payload is not a reader or valid buffer
         * @throws {$protobuf.util.ProtocolError} If required fields are missing
         */
        Signature.decode = function decode(reader, length) {
            if (!(reader instanceof $Reader))
                reader = $Reader.create(reader);
            let end = length === undefined ? reader.len : reader.pos + length, message = new $root.frames.Signature();
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
         * @memberof frames.Signature
         * @static
         * @param {$protobuf.Reader|Uint8Array} reader Reader or buffer to decode from
         * @returns {frames.Signature} Signature
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
         * @memberof frames.Signature
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
         * @memberof frames.Signature
         * @static
         * @param {Object.<string,*>} object Plain object
         * @returns {frames.Signature} Signature
         */
        Signature.fromObject = function fromObject(object) {
            if (object instanceof $root.frames.Signature)
                return object;
            let message = new $root.frames.Signature();
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
         * @memberof frames.Signature
         * @static
         * @param {frames.Signature} message Signature
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
         * @memberof frames.Signature
         * @instance
         * @returns {Object.<string,*>} JSON object
         */
        Signature.prototype.toJSON = function toJSON() {
            return this.constructor.toObject(this, $protobuf.util.toJSONOptions);
        };

        /**
         * Gets the default type url for Signature
         * @function getTypeUrl
         * @memberof frames.Signature
         * @static
         * @param {string} [typeUrlPrefix] your custom typeUrlPrefix(default "type.googleapis.com")
         * @returns {string} The default type url
         */
        Signature.getTypeUrl = function getTypeUrl(typeUrlPrefix) {
            if (typeUrlPrefix === undefined) {
                typeUrlPrefix = "type.googleapis.com";
            }
            return typeUrlPrefix + "/frames.Signature";
        };

        return Signature;
    })();

    frames.PublicKey = (function() {

        /**
         * Properties of a PublicKey.
         * @memberof frames
         * @interface IPublicKey
         * @property {Uint8Array|null} [Value] PublicKey Value
         */

        /**
         * Constructs a new PublicKey.
         * @memberof frames
         * @classdesc Represents a PublicKey.
         * @implements IPublicKey
         * @constructor
         * @param {frames.IPublicKey=} [properties] Properties to set
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
         * @memberof frames.PublicKey
         * @instance
         */
        PublicKey.prototype.Value = $util.newBuffer([]);

        /**
         * Creates a new PublicKey instance using the specified properties.
         * @function create
         * @memberof frames.PublicKey
         * @static
         * @param {frames.IPublicKey=} [properties] Properties to set
         * @returns {frames.PublicKey} PublicKey instance
         */
        PublicKey.create = function create(properties) {
            return new PublicKey(properties);
        };

        /**
         * Encodes the specified PublicKey message. Does not implicitly {@link frames.PublicKey.verify|verify} messages.
         * @function encode
         * @memberof frames.PublicKey
         * @static
         * @param {frames.IPublicKey} message PublicKey message or plain object to encode
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
         * Encodes the specified PublicKey message, length delimited. Does not implicitly {@link frames.PublicKey.verify|verify} messages.
         * @function encodeDelimited
         * @memberof frames.PublicKey
         * @static
         * @param {frames.IPublicKey} message PublicKey message or plain object to encode
         * @param {$protobuf.Writer} [writer] Writer to encode to
         * @returns {$protobuf.Writer} Writer
         */
        PublicKey.encodeDelimited = function encodeDelimited(message, writer) {
            return this.encode(message, writer).ldelim();
        };

        /**
         * Decodes a PublicKey message from the specified reader or buffer.
         * @function decode
         * @memberof frames.PublicKey
         * @static
         * @param {$protobuf.Reader|Uint8Array} reader Reader or buffer to decode from
         * @param {number} [length] Message length if known beforehand
         * @returns {frames.PublicKey} PublicKey
         * @throws {Error} If the payload is not a reader or valid buffer
         * @throws {$protobuf.util.ProtocolError} If required fields are missing
         */
        PublicKey.decode = function decode(reader, length) {
            if (!(reader instanceof $Reader))
                reader = $Reader.create(reader);
            let end = length === undefined ? reader.len : reader.pos + length, message = new $root.frames.PublicKey();
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
         * @memberof frames.PublicKey
         * @static
         * @param {$protobuf.Reader|Uint8Array} reader Reader or buffer to decode from
         * @returns {frames.PublicKey} PublicKey
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
         * @memberof frames.PublicKey
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
         * @memberof frames.PublicKey
         * @static
         * @param {Object.<string,*>} object Plain object
         * @returns {frames.PublicKey} PublicKey
         */
        PublicKey.fromObject = function fromObject(object) {
            if (object instanceof $root.frames.PublicKey)
                return object;
            let message = new $root.frames.PublicKey();
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
         * @memberof frames.PublicKey
         * @static
         * @param {frames.PublicKey} message PublicKey
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
         * @memberof frames.PublicKey
         * @instance
         * @returns {Object.<string,*>} JSON object
         */
        PublicKey.prototype.toJSON = function toJSON() {
            return this.constructor.toObject(this, $protobuf.util.toJSONOptions);
        };

        /**
         * Gets the default type url for PublicKey
         * @function getTypeUrl
         * @memberof frames.PublicKey
         * @static
         * @param {string} [typeUrlPrefix] your custom typeUrlPrefix(default "type.googleapis.com")
         * @returns {string} The default type url
         */
        PublicKey.getTypeUrl = function getTypeUrl(typeUrlPrefix) {
            if (typeUrlPrefix === undefined) {
                typeUrlPrefix = "type.googleapis.com";
            }
            return typeUrlPrefix + "/frames.PublicKey";
        };

        return PublicKey;
    })();

    frames.PaymentRequest = (function() {

        /**
         * Properties of a PaymentRequest.
         * @memberof frames
         * @interface IPaymentRequest
         * @property {string|null} [Value] PaymentRequest Value
         */

        /**
         * Constructs a new PaymentRequest.
         * @memberof frames
         * @classdesc Represents a PaymentRequest.
         * @implements IPaymentRequest
         * @constructor
         * @param {frames.IPaymentRequest=} [properties] Properties to set
         */
        function PaymentRequest(properties) {
            if (properties)
                for (let keys = Object.keys(properties), i = 0; i < keys.length; ++i)
                    if (properties[keys[i]] != null)
                        this[keys[i]] = properties[keys[i]];
        }

        /**
         * PaymentRequest Value.
         * @member {string} Value
         * @memberof frames.PaymentRequest
         * @instance
         */
        PaymentRequest.prototype.Value = "";

        /**
         * Creates a new PaymentRequest instance using the specified properties.
         * @function create
         * @memberof frames.PaymentRequest
         * @static
         * @param {frames.IPaymentRequest=} [properties] Properties to set
         * @returns {frames.PaymentRequest} PaymentRequest instance
         */
        PaymentRequest.create = function create(properties) {
            return new PaymentRequest(properties);
        };

        /**
         * Encodes the specified PaymentRequest message. Does not implicitly {@link frames.PaymentRequest.verify|verify} messages.
         * @function encode
         * @memberof frames.PaymentRequest
         * @static
         * @param {frames.IPaymentRequest} message PaymentRequest message or plain object to encode
         * @param {$protobuf.Writer} [writer] Writer to encode to
         * @returns {$protobuf.Writer} Writer
         */
        PaymentRequest.encode = function encode(message, writer) {
            if (!writer)
                writer = $Writer.create();
            if (message.Value != null && Object.hasOwnProperty.call(message, "Value"))
                writer.uint32(/* id 1, wireType 2 =*/10).string(message.Value);
            return writer;
        };

        /**
         * Encodes the specified PaymentRequest message, length delimited. Does not implicitly {@link frames.PaymentRequest.verify|verify} messages.
         * @function encodeDelimited
         * @memberof frames.PaymentRequest
         * @static
         * @param {frames.IPaymentRequest} message PaymentRequest message or plain object to encode
         * @param {$protobuf.Writer} [writer] Writer to encode to
         * @returns {$protobuf.Writer} Writer
         */
        PaymentRequest.encodeDelimited = function encodeDelimited(message, writer) {
            return this.encode(message, writer).ldelim();
        };

        /**
         * Decodes a PaymentRequest message from the specified reader or buffer.
         * @function decode
         * @memberof frames.PaymentRequest
         * @static
         * @param {$protobuf.Reader|Uint8Array} reader Reader or buffer to decode from
         * @param {number} [length] Message length if known beforehand
         * @returns {frames.PaymentRequest} PaymentRequest
         * @throws {Error} If the payload is not a reader or valid buffer
         * @throws {$protobuf.util.ProtocolError} If required fields are missing
         */
        PaymentRequest.decode = function decode(reader, length) {
            if (!(reader instanceof $Reader))
                reader = $Reader.create(reader);
            let end = length === undefined ? reader.len : reader.pos + length, message = new $root.frames.PaymentRequest();
            while (reader.pos < end) {
                let tag = reader.uint32();
                switch (tag >>> 3) {
                case 1: {
                        message.Value = reader.string();
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
         * Decodes a PaymentRequest message from the specified reader or buffer, length delimited.
         * @function decodeDelimited
         * @memberof frames.PaymentRequest
         * @static
         * @param {$protobuf.Reader|Uint8Array} reader Reader or buffer to decode from
         * @returns {frames.PaymentRequest} PaymentRequest
         * @throws {Error} If the payload is not a reader or valid buffer
         * @throws {$protobuf.util.ProtocolError} If required fields are missing
         */
        PaymentRequest.decodeDelimited = function decodeDelimited(reader) {
            if (!(reader instanceof $Reader))
                reader = new $Reader(reader);
            return this.decode(reader, reader.uint32());
        };

        /**
         * Verifies a PaymentRequest message.
         * @function verify
         * @memberof frames.PaymentRequest
         * @static
         * @param {Object.<string,*>} message Plain object to verify
         * @returns {string|null} `null` if valid, otherwise the reason why it is not
         */
        PaymentRequest.verify = function verify(message) {
            if (typeof message !== "object" || message === null)
                return "object expected";
            if (message.Value != null && message.hasOwnProperty("Value"))
                if (!$util.isString(message.Value))
                    return "Value: string expected";
            return null;
        };

        /**
         * Creates a PaymentRequest message from a plain object. Also converts values to their respective internal types.
         * @function fromObject
         * @memberof frames.PaymentRequest
         * @static
         * @param {Object.<string,*>} object Plain object
         * @returns {frames.PaymentRequest} PaymentRequest
         */
        PaymentRequest.fromObject = function fromObject(object) {
            if (object instanceof $root.frames.PaymentRequest)
                return object;
            let message = new $root.frames.PaymentRequest();
            if (object.Value != null)
                message.Value = String(object.Value);
            return message;
        };

        /**
         * Creates a plain object from a PaymentRequest message. Also converts values to other types if specified.
         * @function toObject
         * @memberof frames.PaymentRequest
         * @static
         * @param {frames.PaymentRequest} message PaymentRequest
         * @param {$protobuf.IConversionOptions} [options] Conversion options
         * @returns {Object.<string,*>} Plain object
         */
        PaymentRequest.toObject = function toObject(message, options) {
            if (!options)
                options = {};
            let object = {};
            if (options.defaults)
                object.Value = "";
            if (message.Value != null && message.hasOwnProperty("Value"))
                object.Value = message.Value;
            return object;
        };

        /**
         * Converts this PaymentRequest to JSON.
         * @function toJSON
         * @memberof frames.PaymentRequest
         * @instance
         * @returns {Object.<string,*>} JSON object
         */
        PaymentRequest.prototype.toJSON = function toJSON() {
            return this.constructor.toObject(this, $protobuf.util.toJSONOptions);
        };

        /**
         * Gets the default type url for PaymentRequest
         * @function getTypeUrl
         * @memberof frames.PaymentRequest
         * @static
         * @param {string} [typeUrlPrefix] your custom typeUrlPrefix(default "type.googleapis.com")
         * @returns {string} The default type url
         */
        PaymentRequest.getTypeUrl = function getTypeUrl(typeUrlPrefix) {
            if (typeUrlPrefix === undefined) {
                typeUrlPrefix = "type.googleapis.com";
            }
            return typeUrlPrefix + "/frames.PaymentRequest";
        };

        return PaymentRequest;
    })();

    frames.PaymentHash = (function() {

        /**
         * Properties of a PaymentHash.
         * @memberof frames
         * @interface IPaymentHash
         * @property {Uint8Array|null} [Value] PaymentHash Value
         */

        /**
         * Constructs a new PaymentHash.
         * @memberof frames
         * @classdesc Represents a PaymentHash.
         * @implements IPaymentHash
         * @constructor
         * @param {frames.IPaymentHash=} [properties] Properties to set
         */
        function PaymentHash(properties) {
            if (properties)
                for (let keys = Object.keys(properties), i = 0; i < keys.length; ++i)
                    if (properties[keys[i]] != null)
                        this[keys[i]] = properties[keys[i]];
        }

        /**
         * PaymentHash Value.
         * @member {Uint8Array} Value
         * @memberof frames.PaymentHash
         * @instance
         */
        PaymentHash.prototype.Value = $util.newBuffer([]);

        /**
         * Creates a new PaymentHash instance using the specified properties.
         * @function create
         * @memberof frames.PaymentHash
         * @static
         * @param {frames.IPaymentHash=} [properties] Properties to set
         * @returns {frames.PaymentHash} PaymentHash instance
         */
        PaymentHash.create = function create(properties) {
            return new PaymentHash(properties);
        };

        /**
         * Encodes the specified PaymentHash message. Does not implicitly {@link frames.PaymentHash.verify|verify} messages.
         * @function encode
         * @memberof frames.PaymentHash
         * @static
         * @param {frames.IPaymentHash} message PaymentHash message or plain object to encode
         * @param {$protobuf.Writer} [writer] Writer to encode to
         * @returns {$protobuf.Writer} Writer
         */
        PaymentHash.encode = function encode(message, writer) {
            if (!writer)
                writer = $Writer.create();
            if (message.Value != null && Object.hasOwnProperty.call(message, "Value"))
                writer.uint32(/* id 1, wireType 2 =*/10).bytes(message.Value);
            return writer;
        };

        /**
         * Encodes the specified PaymentHash message, length delimited. Does not implicitly {@link frames.PaymentHash.verify|verify} messages.
         * @function encodeDelimited
         * @memberof frames.PaymentHash
         * @static
         * @param {frames.IPaymentHash} message PaymentHash message or plain object to encode
         * @param {$protobuf.Writer} [writer] Writer to encode to
         * @returns {$protobuf.Writer} Writer
         */
        PaymentHash.encodeDelimited = function encodeDelimited(message, writer) {
            return this.encode(message, writer).ldelim();
        };

        /**
         * Decodes a PaymentHash message from the specified reader or buffer.
         * @function decode
         * @memberof frames.PaymentHash
         * @static
         * @param {$protobuf.Reader|Uint8Array} reader Reader or buffer to decode from
         * @param {number} [length] Message length if known beforehand
         * @returns {frames.PaymentHash} PaymentHash
         * @throws {Error} If the payload is not a reader or valid buffer
         * @throws {$protobuf.util.ProtocolError} If required fields are missing
         */
        PaymentHash.decode = function decode(reader, length) {
            if (!(reader instanceof $Reader))
                reader = $Reader.create(reader);
            let end = length === undefined ? reader.len : reader.pos + length, message = new $root.frames.PaymentHash();
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
         * Decodes a PaymentHash message from the specified reader or buffer, length delimited.
         * @function decodeDelimited
         * @memberof frames.PaymentHash
         * @static
         * @param {$protobuf.Reader|Uint8Array} reader Reader or buffer to decode from
         * @returns {frames.PaymentHash} PaymentHash
         * @throws {Error} If the payload is not a reader or valid buffer
         * @throws {$protobuf.util.ProtocolError} If required fields are missing
         */
        PaymentHash.decodeDelimited = function decodeDelimited(reader) {
            if (!(reader instanceof $Reader))
                reader = new $Reader(reader);
            return this.decode(reader, reader.uint32());
        };

        /**
         * Verifies a PaymentHash message.
         * @function verify
         * @memberof frames.PaymentHash
         * @static
         * @param {Object.<string,*>} message Plain object to verify
         * @returns {string|null} `null` if valid, otherwise the reason why it is not
         */
        PaymentHash.verify = function verify(message) {
            if (typeof message !== "object" || message === null)
                return "object expected";
            if (message.Value != null && message.hasOwnProperty("Value"))
                if (!(message.Value && typeof message.Value.length === "number" || $util.isString(message.Value)))
                    return "Value: buffer expected";
            return null;
        };

        /**
         * Creates a PaymentHash message from a plain object. Also converts values to their respective internal types.
         * @function fromObject
         * @memberof frames.PaymentHash
         * @static
         * @param {Object.<string,*>} object Plain object
         * @returns {frames.PaymentHash} PaymentHash
         */
        PaymentHash.fromObject = function fromObject(object) {
            if (object instanceof $root.frames.PaymentHash)
                return object;
            let message = new $root.frames.PaymentHash();
            if (object.Value != null)
                if (typeof object.Value === "string")
                    $util.base64.decode(object.Value, message.Value = $util.newBuffer($util.base64.length(object.Value)), 0);
                else if (object.Value.length >= 0)
                    message.Value = object.Value;
            return message;
        };

        /**
         * Creates a plain object from a PaymentHash message. Also converts values to other types if specified.
         * @function toObject
         * @memberof frames.PaymentHash
         * @static
         * @param {frames.PaymentHash} message PaymentHash
         * @param {$protobuf.IConversionOptions} [options] Conversion options
         * @returns {Object.<string,*>} Plain object
         */
        PaymentHash.toObject = function toObject(message, options) {
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
         * Converts this PaymentHash to JSON.
         * @function toJSON
         * @memberof frames.PaymentHash
         * @instance
         * @returns {Object.<string,*>} JSON object
         */
        PaymentHash.prototype.toJSON = function toJSON() {
            return this.constructor.toObject(this, $protobuf.util.toJSONOptions);
        };

        /**
         * Gets the default type url for PaymentHash
         * @function getTypeUrl
         * @memberof frames.PaymentHash
         * @static
         * @param {string} [typeUrlPrefix] your custom typeUrlPrefix(default "type.googleapis.com")
         * @returns {string} The default type url
         */
        PaymentHash.getTypeUrl = function getTypeUrl(typeUrlPrefix) {
            if (typeUrlPrefix === undefined) {
                typeUrlPrefix = "type.googleapis.com";
            }
            return typeUrlPrefix + "/frames.PaymentHash";
        };

        return PaymentHash;
    })();

    frames.Satoshis = (function() {

        /**
         * Properties of a Satoshis.
         * @memberof frames
         * @interface ISatoshis
         * @property {number|Long|null} [Value] Satoshis Value
         */

        /**
         * Constructs a new Satoshis.
         * @memberof frames
         * @classdesc Represents a Satoshis.
         * @implements ISatoshis
         * @constructor
         * @param {frames.ISatoshis=} [properties] Properties to set
         */
        function Satoshis(properties) {
            if (properties)
                for (let keys = Object.keys(properties), i = 0; i < keys.length; ++i)
                    if (properties[keys[i]] != null)
                        this[keys[i]] = properties[keys[i]];
        }

        /**
         * Satoshis Value.
         * @member {number|Long} Value
         * @memberof frames.Satoshis
         * @instance
         */
        Satoshis.prototype.Value = $util.Long ? $util.Long.fromBits(0,0,false) : 0;

        /**
         * Creates a new Satoshis instance using the specified properties.
         * @function create
         * @memberof frames.Satoshis
         * @static
         * @param {frames.ISatoshis=} [properties] Properties to set
         * @returns {frames.Satoshis} Satoshis instance
         */
        Satoshis.create = function create(properties) {
            return new Satoshis(properties);
        };

        /**
         * Encodes the specified Satoshis message. Does not implicitly {@link frames.Satoshis.verify|verify} messages.
         * @function encode
         * @memberof frames.Satoshis
         * @static
         * @param {frames.ISatoshis} message Satoshis message or plain object to encode
         * @param {$protobuf.Writer} [writer] Writer to encode to
         * @returns {$protobuf.Writer} Writer
         */
        Satoshis.encode = function encode(message, writer) {
            if (!writer)
                writer = $Writer.create();
            if (message.Value != null && Object.hasOwnProperty.call(message, "Value"))
                writer.uint32(/* id 1, wireType 0 =*/8).int64(message.Value);
            return writer;
        };

        /**
         * Encodes the specified Satoshis message, length delimited. Does not implicitly {@link frames.Satoshis.verify|verify} messages.
         * @function encodeDelimited
         * @memberof frames.Satoshis
         * @static
         * @param {frames.ISatoshis} message Satoshis message or plain object to encode
         * @param {$protobuf.Writer} [writer] Writer to encode to
         * @returns {$protobuf.Writer} Writer
         */
        Satoshis.encodeDelimited = function encodeDelimited(message, writer) {
            return this.encode(message, writer).ldelim();
        };

        /**
         * Decodes a Satoshis message from the specified reader or buffer.
         * @function decode
         * @memberof frames.Satoshis
         * @static
         * @param {$protobuf.Reader|Uint8Array} reader Reader or buffer to decode from
         * @param {number} [length] Message length if known beforehand
         * @returns {frames.Satoshis} Satoshis
         * @throws {Error} If the payload is not a reader or valid buffer
         * @throws {$protobuf.util.ProtocolError} If required fields are missing
         */
        Satoshis.decode = function decode(reader, length) {
            if (!(reader instanceof $Reader))
                reader = $Reader.create(reader);
            let end = length === undefined ? reader.len : reader.pos + length, message = new $root.frames.Satoshis();
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
         * Decodes a Satoshis message from the specified reader or buffer, length delimited.
         * @function decodeDelimited
         * @memberof frames.Satoshis
         * @static
         * @param {$protobuf.Reader|Uint8Array} reader Reader or buffer to decode from
         * @returns {frames.Satoshis} Satoshis
         * @throws {Error} If the payload is not a reader or valid buffer
         * @throws {$protobuf.util.ProtocolError} If required fields are missing
         */
        Satoshis.decodeDelimited = function decodeDelimited(reader) {
            if (!(reader instanceof $Reader))
                reader = new $Reader(reader);
            return this.decode(reader, reader.uint32());
        };

        /**
         * Verifies a Satoshis message.
         * @function verify
         * @memberof frames.Satoshis
         * @static
         * @param {Object.<string,*>} message Plain object to verify
         * @returns {string|null} `null` if valid, otherwise the reason why it is not
         */
        Satoshis.verify = function verify(message) {
            if (typeof message !== "object" || message === null)
                return "object expected";
            if (message.Value != null && message.hasOwnProperty("Value"))
                if (!$util.isInteger(message.Value) && !(message.Value && $util.isInteger(message.Value.low) && $util.isInteger(message.Value.high)))
                    return "Value: integer|Long expected";
            return null;
        };

        /**
         * Creates a Satoshis message from a plain object. Also converts values to their respective internal types.
         * @function fromObject
         * @memberof frames.Satoshis
         * @static
         * @param {Object.<string,*>} object Plain object
         * @returns {frames.Satoshis} Satoshis
         */
        Satoshis.fromObject = function fromObject(object) {
            if (object instanceof $root.frames.Satoshis)
                return object;
            let message = new $root.frames.Satoshis();
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
         * Creates a plain object from a Satoshis message. Also converts values to other types if specified.
         * @function toObject
         * @memberof frames.Satoshis
         * @static
         * @param {frames.Satoshis} message Satoshis
         * @param {$protobuf.IConversionOptions} [options] Conversion options
         * @returns {Object.<string,*>} Plain object
         */
        Satoshis.toObject = function toObject(message, options) {
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
         * Converts this Satoshis to JSON.
         * @function toJSON
         * @memberof frames.Satoshis
         * @instance
         * @returns {Object.<string,*>} JSON object
         */
        Satoshis.prototype.toJSON = function toJSON() {
            return this.constructor.toObject(this, $protobuf.util.toJSONOptions);
        };

        /**
         * Gets the default type url for Satoshis
         * @function getTypeUrl
         * @memberof frames.Satoshis
         * @static
         * @param {string} [typeUrlPrefix] your custom typeUrlPrefix(default "type.googleapis.com")
         * @returns {string} The default type url
         */
        Satoshis.getTypeUrl = function getTypeUrl(typeUrlPrefix) {
            if (typeUrlPrefix === undefined) {
                typeUrlPrefix = "type.googleapis.com";
            }
            return typeUrlPrefix + "/frames.Satoshis";
        };

        return Satoshis;
    })();

    frames.EncryptedData = (function() {

        /**
         * Properties of an EncryptedData.
         * @memberof frames
         * @interface IEncryptedData
         * @property {Uint8Array|null} [Value] EncryptedData Value
         */

        /**
         * Constructs a new EncryptedData.
         * @memberof frames
         * @classdesc Represents an EncryptedData.
         * @implements IEncryptedData
         * @constructor
         * @param {frames.IEncryptedData=} [properties] Properties to set
         */
        function EncryptedData(properties) {
            if (properties)
                for (let keys = Object.keys(properties), i = 0; i < keys.length; ++i)
                    if (properties[keys[i]] != null)
                        this[keys[i]] = properties[keys[i]];
        }

        /**
         * EncryptedData Value.
         * @member {Uint8Array} Value
         * @memberof frames.EncryptedData
         * @instance
         */
        EncryptedData.prototype.Value = $util.newBuffer([]);

        /**
         * Creates a new EncryptedData instance using the specified properties.
         * @function create
         * @memberof frames.EncryptedData
         * @static
         * @param {frames.IEncryptedData=} [properties] Properties to set
         * @returns {frames.EncryptedData} EncryptedData instance
         */
        EncryptedData.create = function create(properties) {
            return new EncryptedData(properties);
        };

        /**
         * Encodes the specified EncryptedData message. Does not implicitly {@link frames.EncryptedData.verify|verify} messages.
         * @function encode
         * @memberof frames.EncryptedData
         * @static
         * @param {frames.IEncryptedData} message EncryptedData message or plain object to encode
         * @param {$protobuf.Writer} [writer] Writer to encode to
         * @returns {$protobuf.Writer} Writer
         */
        EncryptedData.encode = function encode(message, writer) {
            if (!writer)
                writer = $Writer.create();
            if (message.Value != null && Object.hasOwnProperty.call(message, "Value"))
                writer.uint32(/* id 1, wireType 2 =*/10).bytes(message.Value);
            return writer;
        };

        /**
         * Encodes the specified EncryptedData message, length delimited. Does not implicitly {@link frames.EncryptedData.verify|verify} messages.
         * @function encodeDelimited
         * @memberof frames.EncryptedData
         * @static
         * @param {frames.IEncryptedData} message EncryptedData message or plain object to encode
         * @param {$protobuf.Writer} [writer] Writer to encode to
         * @returns {$protobuf.Writer} Writer
         */
        EncryptedData.encodeDelimited = function encodeDelimited(message, writer) {
            return this.encode(message, writer).ldelim();
        };

        /**
         * Decodes an EncryptedData message from the specified reader or buffer.
         * @function decode
         * @memberof frames.EncryptedData
         * @static
         * @param {$protobuf.Reader|Uint8Array} reader Reader or buffer to decode from
         * @param {number} [length] Message length if known beforehand
         * @returns {frames.EncryptedData} EncryptedData
         * @throws {Error} If the payload is not a reader or valid buffer
         * @throws {$protobuf.util.ProtocolError} If required fields are missing
         */
        EncryptedData.decode = function decode(reader, length) {
            if (!(reader instanceof $Reader))
                reader = $Reader.create(reader);
            let end = length === undefined ? reader.len : reader.pos + length, message = new $root.frames.EncryptedData();
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
         * Decodes an EncryptedData message from the specified reader or buffer, length delimited.
         * @function decodeDelimited
         * @memberof frames.EncryptedData
         * @static
         * @param {$protobuf.Reader|Uint8Array} reader Reader or buffer to decode from
         * @returns {frames.EncryptedData} EncryptedData
         * @throws {Error} If the payload is not a reader or valid buffer
         * @throws {$protobuf.util.ProtocolError} If required fields are missing
         */
        EncryptedData.decodeDelimited = function decodeDelimited(reader) {
            if (!(reader instanceof $Reader))
                reader = new $Reader(reader);
            return this.decode(reader, reader.uint32());
        };

        /**
         * Verifies an EncryptedData message.
         * @function verify
         * @memberof frames.EncryptedData
         * @static
         * @param {Object.<string,*>} message Plain object to verify
         * @returns {string|null} `null` if valid, otherwise the reason why it is not
         */
        EncryptedData.verify = function verify(message) {
            if (typeof message !== "object" || message === null)
                return "object expected";
            if (message.Value != null && message.hasOwnProperty("Value"))
                if (!(message.Value && typeof message.Value.length === "number" || $util.isString(message.Value)))
                    return "Value: buffer expected";
            return null;
        };

        /**
         * Creates an EncryptedData message from a plain object. Also converts values to their respective internal types.
         * @function fromObject
         * @memberof frames.EncryptedData
         * @static
         * @param {Object.<string,*>} object Plain object
         * @returns {frames.EncryptedData} EncryptedData
         */
        EncryptedData.fromObject = function fromObject(object) {
            if (object instanceof $root.frames.EncryptedData)
                return object;
            let message = new $root.frames.EncryptedData();
            if (object.Value != null)
                if (typeof object.Value === "string")
                    $util.base64.decode(object.Value, message.Value = $util.newBuffer($util.base64.length(object.Value)), 0);
                else if (object.Value.length >= 0)
                    message.Value = object.Value;
            return message;
        };

        /**
         * Creates a plain object from an EncryptedData message. Also converts values to other types if specified.
         * @function toObject
         * @memberof frames.EncryptedData
         * @static
         * @param {frames.EncryptedData} message EncryptedData
         * @param {$protobuf.IConversionOptions} [options] Conversion options
         * @returns {Object.<string,*>} Plain object
         */
        EncryptedData.toObject = function toObject(message, options) {
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
         * Converts this EncryptedData to JSON.
         * @function toJSON
         * @memberof frames.EncryptedData
         * @instance
         * @returns {Object.<string,*>} JSON object
         */
        EncryptedData.prototype.toJSON = function toJSON() {
            return this.constructor.toObject(this, $protobuf.util.toJSONOptions);
        };

        /**
         * Gets the default type url for EncryptedData
         * @function getTypeUrl
         * @memberof frames.EncryptedData
         * @static
         * @param {string} [typeUrlPrefix] your custom typeUrlPrefix(default "type.googleapis.com")
         * @returns {string} The default type url
         */
        EncryptedData.getTypeUrl = function getTypeUrl(typeUrlPrefix) {
            if (typeUrlPrefix === undefined) {
                typeUrlPrefix = "type.googleapis.com";
            }
            return typeUrlPrefix + "/frames.EncryptedData";
        };

        return EncryptedData;
    })();

    frames.CryptographicHash = (function() {

        /**
         * Properties of a CryptographicHash.
         * @memberof frames
         * @interface ICryptographicHash
         * @property {Uint8Array|null} [Value] CryptographicHash Value
         */

        /**
         * Constructs a new CryptographicHash.
         * @memberof frames
         * @classdesc Represents a CryptographicHash.
         * @implements ICryptographicHash
         * @constructor
         * @param {frames.ICryptographicHash=} [properties] Properties to set
         */
        function CryptographicHash(properties) {
            if (properties)
                for (let keys = Object.keys(properties), i = 0; i < keys.length; ++i)
                    if (properties[keys[i]] != null)
                        this[keys[i]] = properties[keys[i]];
        }

        /**
         * CryptographicHash Value.
         * @member {Uint8Array} Value
         * @memberof frames.CryptographicHash
         * @instance
         */
        CryptographicHash.prototype.Value = $util.newBuffer([]);

        /**
         * Creates a new CryptographicHash instance using the specified properties.
         * @function create
         * @memberof frames.CryptographicHash
         * @static
         * @param {frames.ICryptographicHash=} [properties] Properties to set
         * @returns {frames.CryptographicHash} CryptographicHash instance
         */
        CryptographicHash.create = function create(properties) {
            return new CryptographicHash(properties);
        };

        /**
         * Encodes the specified CryptographicHash message. Does not implicitly {@link frames.CryptographicHash.verify|verify} messages.
         * @function encode
         * @memberof frames.CryptographicHash
         * @static
         * @param {frames.ICryptographicHash} message CryptographicHash message or plain object to encode
         * @param {$protobuf.Writer} [writer] Writer to encode to
         * @returns {$protobuf.Writer} Writer
         */
        CryptographicHash.encode = function encode(message, writer) {
            if (!writer)
                writer = $Writer.create();
            if (message.Value != null && Object.hasOwnProperty.call(message, "Value"))
                writer.uint32(/* id 1, wireType 2 =*/10).bytes(message.Value);
            return writer;
        };

        /**
         * Encodes the specified CryptographicHash message, length delimited. Does not implicitly {@link frames.CryptographicHash.verify|verify} messages.
         * @function encodeDelimited
         * @memberof frames.CryptographicHash
         * @static
         * @param {frames.ICryptographicHash} message CryptographicHash message or plain object to encode
         * @param {$protobuf.Writer} [writer] Writer to encode to
         * @returns {$protobuf.Writer} Writer
         */
        CryptographicHash.encodeDelimited = function encodeDelimited(message, writer) {
            return this.encode(message, writer).ldelim();
        };

        /**
         * Decodes a CryptographicHash message from the specified reader or buffer.
         * @function decode
         * @memberof frames.CryptographicHash
         * @static
         * @param {$protobuf.Reader|Uint8Array} reader Reader or buffer to decode from
         * @param {number} [length] Message length if known beforehand
         * @returns {frames.CryptographicHash} CryptographicHash
         * @throws {Error} If the payload is not a reader or valid buffer
         * @throws {$protobuf.util.ProtocolError} If required fields are missing
         */
        CryptographicHash.decode = function decode(reader, length) {
            if (!(reader instanceof $Reader))
                reader = $Reader.create(reader);
            let end = length === undefined ? reader.len : reader.pos + length, message = new $root.frames.CryptographicHash();
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
         * Decodes a CryptographicHash message from the specified reader or buffer, length delimited.
         * @function decodeDelimited
         * @memberof frames.CryptographicHash
         * @static
         * @param {$protobuf.Reader|Uint8Array} reader Reader or buffer to decode from
         * @returns {frames.CryptographicHash} CryptographicHash
         * @throws {Error} If the payload is not a reader or valid buffer
         * @throws {$protobuf.util.ProtocolError} If required fields are missing
         */
        CryptographicHash.decodeDelimited = function decodeDelimited(reader) {
            if (!(reader instanceof $Reader))
                reader = new $Reader(reader);
            return this.decode(reader, reader.uint32());
        };

        /**
         * Verifies a CryptographicHash message.
         * @function verify
         * @memberof frames.CryptographicHash
         * @static
         * @param {Object.<string,*>} message Plain object to verify
         * @returns {string|null} `null` if valid, otherwise the reason why it is not
         */
        CryptographicHash.verify = function verify(message) {
            if (typeof message !== "object" || message === null)
                return "object expected";
            if (message.Value != null && message.hasOwnProperty("Value"))
                if (!(message.Value && typeof message.Value.length === "number" || $util.isString(message.Value)))
                    return "Value: buffer expected";
            return null;
        };

        /**
         * Creates a CryptographicHash message from a plain object. Also converts values to their respective internal types.
         * @function fromObject
         * @memberof frames.CryptographicHash
         * @static
         * @param {Object.<string,*>} object Plain object
         * @returns {frames.CryptographicHash} CryptographicHash
         */
        CryptographicHash.fromObject = function fromObject(object) {
            if (object instanceof $root.frames.CryptographicHash)
                return object;
            let message = new $root.frames.CryptographicHash();
            if (object.Value != null)
                if (typeof object.Value === "string")
                    $util.base64.decode(object.Value, message.Value = $util.newBuffer($util.base64.length(object.Value)), 0);
                else if (object.Value.length >= 0)
                    message.Value = object.Value;
            return message;
        };

        /**
         * Creates a plain object from a CryptographicHash message. Also converts values to other types if specified.
         * @function toObject
         * @memberof frames.CryptographicHash
         * @static
         * @param {frames.CryptographicHash} message CryptographicHash
         * @param {$protobuf.IConversionOptions} [options] Conversion options
         * @returns {Object.<string,*>} Plain object
         */
        CryptographicHash.toObject = function toObject(message, options) {
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
         * Converts this CryptographicHash to JSON.
         * @function toJSON
         * @memberof frames.CryptographicHash
         * @instance
         * @returns {Object.<string,*>} JSON object
         */
        CryptographicHash.prototype.toJSON = function toJSON() {
            return this.constructor.toObject(this, $protobuf.util.toJSONOptions);
        };

        /**
         * Gets the default type url for CryptographicHash
         * @function getTypeUrl
         * @memberof frames.CryptographicHash
         * @static
         * @param {string} [typeUrlPrefix] your custom typeUrlPrefix(default "type.googleapis.com")
         * @returns {string} The default type url
         */
        CryptographicHash.getTypeUrl = function getTypeUrl(typeUrlPrefix) {
            if (typeUrlPrefix === undefined) {
                typeUrlPrefix = "type.googleapis.com";
            }
            return typeUrlPrefix + "/frames.CryptographicHash";
        };

        return CryptographicHash;
    })();

    frames.AuthTokenHeader = (function() {

        /**
         * Properties of an AuthTokenHeader.
         * @memberof frames
         * @interface IAuthTokenHeader
         * @property {frames.IUUID|null} [TokenId] AuthTokenHeader TokenId
         * @property {frames.IPublicKey|null} [PublicKey] AuthTokenHeader PublicKey
         * @property {frames.ITimestamp|null} [Timestamp] AuthTokenHeader Timestamp
         */

        /**
         * Constructs a new AuthTokenHeader.
         * @memberof frames
         * @classdesc Represents an AuthTokenHeader.
         * @implements IAuthTokenHeader
         * @constructor
         * @param {frames.IAuthTokenHeader=} [properties] Properties to set
         */
        function AuthTokenHeader(properties) {
            if (properties)
                for (let keys = Object.keys(properties), i = 0; i < keys.length; ++i)
                    if (properties[keys[i]] != null)
                        this[keys[i]] = properties[keys[i]];
        }

        /**
         * AuthTokenHeader TokenId.
         * @member {frames.IUUID|null|undefined} TokenId
         * @memberof frames.AuthTokenHeader
         * @instance
         */
        AuthTokenHeader.prototype.TokenId = null;

        /**
         * AuthTokenHeader PublicKey.
         * @member {frames.IPublicKey|null|undefined} PublicKey
         * @memberof frames.AuthTokenHeader
         * @instance
         */
        AuthTokenHeader.prototype.PublicKey = null;

        /**
         * AuthTokenHeader Timestamp.
         * @member {frames.ITimestamp|null|undefined} Timestamp
         * @memberof frames.AuthTokenHeader
         * @instance
         */
        AuthTokenHeader.prototype.Timestamp = null;

        /**
         * Creates a new AuthTokenHeader instance using the specified properties.
         * @function create
         * @memberof frames.AuthTokenHeader
         * @static
         * @param {frames.IAuthTokenHeader=} [properties] Properties to set
         * @returns {frames.AuthTokenHeader} AuthTokenHeader instance
         */
        AuthTokenHeader.create = function create(properties) {
            return new AuthTokenHeader(properties);
        };

        /**
         * Encodes the specified AuthTokenHeader message. Does not implicitly {@link frames.AuthTokenHeader.verify|verify} messages.
         * @function encode
         * @memberof frames.AuthTokenHeader
         * @static
         * @param {frames.IAuthTokenHeader} message AuthTokenHeader message or plain object to encode
         * @param {$protobuf.Writer} [writer] Writer to encode to
         * @returns {$protobuf.Writer} Writer
         */
        AuthTokenHeader.encode = function encode(message, writer) {
            if (!writer)
                writer = $Writer.create();
            if (message.TokenId != null && Object.hasOwnProperty.call(message, "TokenId"))
                $root.frames.UUID.encode(message.TokenId, writer.uint32(/* id 1, wireType 2 =*/10).fork()).ldelim();
            if (message.PublicKey != null && Object.hasOwnProperty.call(message, "PublicKey"))
                $root.frames.PublicKey.encode(message.PublicKey, writer.uint32(/* id 2, wireType 2 =*/18).fork()).ldelim();
            if (message.Timestamp != null && Object.hasOwnProperty.call(message, "Timestamp"))
                $root.frames.Timestamp.encode(message.Timestamp, writer.uint32(/* id 3, wireType 2 =*/26).fork()).ldelim();
            return writer;
        };

        /**
         * Encodes the specified AuthTokenHeader message, length delimited. Does not implicitly {@link frames.AuthTokenHeader.verify|verify} messages.
         * @function encodeDelimited
         * @memberof frames.AuthTokenHeader
         * @static
         * @param {frames.IAuthTokenHeader} message AuthTokenHeader message or plain object to encode
         * @param {$protobuf.Writer} [writer] Writer to encode to
         * @returns {$protobuf.Writer} Writer
         */
        AuthTokenHeader.encodeDelimited = function encodeDelimited(message, writer) {
            return this.encode(message, writer).ldelim();
        };

        /**
         * Decodes an AuthTokenHeader message from the specified reader or buffer.
         * @function decode
         * @memberof frames.AuthTokenHeader
         * @static
         * @param {$protobuf.Reader|Uint8Array} reader Reader or buffer to decode from
         * @param {number} [length] Message length if known beforehand
         * @returns {frames.AuthTokenHeader} AuthTokenHeader
         * @throws {Error} If the payload is not a reader or valid buffer
         * @throws {$protobuf.util.ProtocolError} If required fields are missing
         */
        AuthTokenHeader.decode = function decode(reader, length) {
            if (!(reader instanceof $Reader))
                reader = $Reader.create(reader);
            let end = length === undefined ? reader.len : reader.pos + length, message = new $root.frames.AuthTokenHeader();
            while (reader.pos < end) {
                let tag = reader.uint32();
                switch (tag >>> 3) {
                case 1: {
                        message.TokenId = $root.frames.UUID.decode(reader, reader.uint32());
                        break;
                    }
                case 2: {
                        message.PublicKey = $root.frames.PublicKey.decode(reader, reader.uint32());
                        break;
                    }
                case 3: {
                        message.Timestamp = $root.frames.Timestamp.decode(reader, reader.uint32());
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
         * @memberof frames.AuthTokenHeader
         * @static
         * @param {$protobuf.Reader|Uint8Array} reader Reader or buffer to decode from
         * @returns {frames.AuthTokenHeader} AuthTokenHeader
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
         * @memberof frames.AuthTokenHeader
         * @static
         * @param {Object.<string,*>} message Plain object to verify
         * @returns {string|null} `null` if valid, otherwise the reason why it is not
         */
        AuthTokenHeader.verify = function verify(message) {
            if (typeof message !== "object" || message === null)
                return "object expected";
            if (message.TokenId != null && message.hasOwnProperty("TokenId")) {
                let error = $root.frames.UUID.verify(message.TokenId);
                if (error)
                    return "TokenId." + error;
            }
            if (message.PublicKey != null && message.hasOwnProperty("PublicKey")) {
                let error = $root.frames.PublicKey.verify(message.PublicKey);
                if (error)
                    return "PublicKey." + error;
            }
            if (message.Timestamp != null && message.hasOwnProperty("Timestamp")) {
                let error = $root.frames.Timestamp.verify(message.Timestamp);
                if (error)
                    return "Timestamp." + error;
            }
            return null;
        };

        /**
         * Creates an AuthTokenHeader message from a plain object. Also converts values to their respective internal types.
         * @function fromObject
         * @memberof frames.AuthTokenHeader
         * @static
         * @param {Object.<string,*>} object Plain object
         * @returns {frames.AuthTokenHeader} AuthTokenHeader
         */
        AuthTokenHeader.fromObject = function fromObject(object) {
            if (object instanceof $root.frames.AuthTokenHeader)
                return object;
            let message = new $root.frames.AuthTokenHeader();
            if (object.TokenId != null) {
                if (typeof object.TokenId !== "object")
                    throw TypeError(".frames.AuthTokenHeader.TokenId: object expected");
                message.TokenId = $root.frames.UUID.fromObject(object.TokenId);
            }
            if (object.PublicKey != null) {
                if (typeof object.PublicKey !== "object")
                    throw TypeError(".frames.AuthTokenHeader.PublicKey: object expected");
                message.PublicKey = $root.frames.PublicKey.fromObject(object.PublicKey);
            }
            if (object.Timestamp != null) {
                if (typeof object.Timestamp !== "object")
                    throw TypeError(".frames.AuthTokenHeader.Timestamp: object expected");
                message.Timestamp = $root.frames.Timestamp.fromObject(object.Timestamp);
            }
            return message;
        };

        /**
         * Creates a plain object from an AuthTokenHeader message. Also converts values to other types if specified.
         * @function toObject
         * @memberof frames.AuthTokenHeader
         * @static
         * @param {frames.AuthTokenHeader} message AuthTokenHeader
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
                object.TokenId = $root.frames.UUID.toObject(message.TokenId, options);
            if (message.PublicKey != null && message.hasOwnProperty("PublicKey"))
                object.PublicKey = $root.frames.PublicKey.toObject(message.PublicKey, options);
            if (message.Timestamp != null && message.hasOwnProperty("Timestamp"))
                object.Timestamp = $root.frames.Timestamp.toObject(message.Timestamp, options);
            return object;
        };

        /**
         * Converts this AuthTokenHeader to JSON.
         * @function toJSON
         * @memberof frames.AuthTokenHeader
         * @instance
         * @returns {Object.<string,*>} JSON object
         */
        AuthTokenHeader.prototype.toJSON = function toJSON() {
            return this.constructor.toObject(this, $protobuf.util.toJSONOptions);
        };

        /**
         * Gets the default type url for AuthTokenHeader
         * @function getTypeUrl
         * @memberof frames.AuthTokenHeader
         * @static
         * @param {string} [typeUrlPrefix] your custom typeUrlPrefix(default "type.googleapis.com")
         * @returns {string} The default type url
         */
        AuthTokenHeader.getTypeUrl = function getTypeUrl(typeUrlPrefix) {
            if (typeUrlPrefix === undefined) {
                typeUrlPrefix = "type.googleapis.com";
            }
            return typeUrlPrefix + "/frames.AuthTokenHeader";
        };

        return AuthTokenHeader;
    })();

    frames.AuthToken = (function() {

        /**
         * Properties of an AuthToken.
         * @memberof frames
         * @interface IAuthToken
         * @property {frames.IAuthTokenHeader|null} [Header] AuthToken Header
         * @property {frames.ISignature|null} [Signature] AuthToken Signature
         */

        /**
         * Constructs a new AuthToken.
         * @memberof frames
         * @classdesc </summary>
         * @implements IAuthToken
         * @constructor
         * @param {frames.IAuthToken=} [properties] Properties to set
         */
        function AuthToken(properties) {
            if (properties)
                for (let keys = Object.keys(properties), i = 0; i < keys.length; ++i)
                    if (properties[keys[i]] != null)
                        this[keys[i]] = properties[keys[i]];
        }

        /**
         * AuthToken Header.
         * @member {frames.IAuthTokenHeader|null|undefined} Header
         * @memberof frames.AuthToken
         * @instance
         */
        AuthToken.prototype.Header = null;

        /**
         * AuthToken Signature.
         * @member {frames.ISignature|null|undefined} Signature
         * @memberof frames.AuthToken
         * @instance
         */
        AuthToken.prototype.Signature = null;

        /**
         * Creates a new AuthToken instance using the specified properties.
         * @function create
         * @memberof frames.AuthToken
         * @static
         * @param {frames.IAuthToken=} [properties] Properties to set
         * @returns {frames.AuthToken} AuthToken instance
         */
        AuthToken.create = function create(properties) {
            return new AuthToken(properties);
        };

        /**
         * Encodes the specified AuthToken message. Does not implicitly {@link frames.AuthToken.verify|verify} messages.
         * @function encode
         * @memberof frames.AuthToken
         * @static
         * @param {frames.IAuthToken} message AuthToken message or plain object to encode
         * @param {$protobuf.Writer} [writer] Writer to encode to
         * @returns {$protobuf.Writer} Writer
         */
        AuthToken.encode = function encode(message, writer) {
            if (!writer)
                writer = $Writer.create();
            if (message.Header != null && Object.hasOwnProperty.call(message, "Header"))
                $root.frames.AuthTokenHeader.encode(message.Header, writer.uint32(/* id 1, wireType 2 =*/10).fork()).ldelim();
            if (message.Signature != null && Object.hasOwnProperty.call(message, "Signature"))
                $root.frames.Signature.encode(message.Signature, writer.uint32(/* id 2, wireType 2 =*/18).fork()).ldelim();
            return writer;
        };

        /**
         * Encodes the specified AuthToken message, length delimited. Does not implicitly {@link frames.AuthToken.verify|verify} messages.
         * @function encodeDelimited
         * @memberof frames.AuthToken
         * @static
         * @param {frames.IAuthToken} message AuthToken message or plain object to encode
         * @param {$protobuf.Writer} [writer] Writer to encode to
         * @returns {$protobuf.Writer} Writer
         */
        AuthToken.encodeDelimited = function encodeDelimited(message, writer) {
            return this.encode(message, writer).ldelim();
        };

        /**
         * Decodes an AuthToken message from the specified reader or buffer.
         * @function decode
         * @memberof frames.AuthToken
         * @static
         * @param {$protobuf.Reader|Uint8Array} reader Reader or buffer to decode from
         * @param {number} [length] Message length if known beforehand
         * @returns {frames.AuthToken} AuthToken
         * @throws {Error} If the payload is not a reader or valid buffer
         * @throws {$protobuf.util.ProtocolError} If required fields are missing
         */
        AuthToken.decode = function decode(reader, length) {
            if (!(reader instanceof $Reader))
                reader = $Reader.create(reader);
            let end = length === undefined ? reader.len : reader.pos + length, message = new $root.frames.AuthToken();
            while (reader.pos < end) {
                let tag = reader.uint32();
                switch (tag >>> 3) {
                case 1: {
                        message.Header = $root.frames.AuthTokenHeader.decode(reader, reader.uint32());
                        break;
                    }
                case 2: {
                        message.Signature = $root.frames.Signature.decode(reader, reader.uint32());
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
         * @memberof frames.AuthToken
         * @static
         * @param {$protobuf.Reader|Uint8Array} reader Reader or buffer to decode from
         * @returns {frames.AuthToken} AuthToken
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
         * @memberof frames.AuthToken
         * @static
         * @param {Object.<string,*>} message Plain object to verify
         * @returns {string|null} `null` if valid, otherwise the reason why it is not
         */
        AuthToken.verify = function verify(message) {
            if (typeof message !== "object" || message === null)
                return "object expected";
            if (message.Header != null && message.hasOwnProperty("Header")) {
                let error = $root.frames.AuthTokenHeader.verify(message.Header);
                if (error)
                    return "Header." + error;
            }
            if (message.Signature != null && message.hasOwnProperty("Signature")) {
                let error = $root.frames.Signature.verify(message.Signature);
                if (error)
                    return "Signature." + error;
            }
            return null;
        };

        /**
         * Creates an AuthToken message from a plain object. Also converts values to their respective internal types.
         * @function fromObject
         * @memberof frames.AuthToken
         * @static
         * @param {Object.<string,*>} object Plain object
         * @returns {frames.AuthToken} AuthToken
         */
        AuthToken.fromObject = function fromObject(object) {
            if (object instanceof $root.frames.AuthToken)
                return object;
            let message = new $root.frames.AuthToken();
            if (object.Header != null) {
                if (typeof object.Header !== "object")
                    throw TypeError(".frames.AuthToken.Header: object expected");
                message.Header = $root.frames.AuthTokenHeader.fromObject(object.Header);
            }
            if (object.Signature != null) {
                if (typeof object.Signature !== "object")
                    throw TypeError(".frames.AuthToken.Signature: object expected");
                message.Signature = $root.frames.Signature.fromObject(object.Signature);
            }
            return message;
        };

        /**
         * Creates a plain object from an AuthToken message. Also converts values to other types if specified.
         * @function toObject
         * @memberof frames.AuthToken
         * @static
         * @param {frames.AuthToken} message AuthToken
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
                object.Header = $root.frames.AuthTokenHeader.toObject(message.Header, options);
            if (message.Signature != null && message.hasOwnProperty("Signature"))
                object.Signature = $root.frames.Signature.toObject(message.Signature, options);
            return object;
        };

        /**
         * Converts this AuthToken to JSON.
         * @function toJSON
         * @memberof frames.AuthToken
         * @instance
         * @returns {Object.<string,*>} JSON object
         */
        AuthToken.prototype.toJSON = function toJSON() {
            return this.constructor.toObject(this, $protobuf.util.toJSONOptions);
        };

        /**
         * Gets the default type url for AuthToken
         * @function getTypeUrl
         * @memberof frames.AuthToken
         * @static
         * @param {string} [typeUrlPrefix] your custom typeUrlPrefix(default "type.googleapis.com")
         * @returns {string} The default type url
         */
        AuthToken.getTypeUrl = function getTypeUrl(typeUrlPrefix) {
            if (typeUrlPrefix === undefined) {
                typeUrlPrefix = "type.googleapis.com";
            }
            return typeUrlPrefix + "/frames.AuthToken";
        };

        return AuthToken;
    })();

    return frames;
})();

module.exports = { frames };