using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPF.Windows.OpenGL
{
    /// <summary>
    ///  AccumOp
    /// </summary>
    public enum AccumOperation : uint
    {
        Accum = OpenGl.GL_ACCUM,
        Load = OpenGl.GL_LOAD,
        Return = OpenGl.GL_RETURN,
        Multiple = OpenGl.GL_MULT,
        Add = OpenGl.GL_ADD
    }

    /// <summary>
    /// The alpha function
    /// </summary>
    public enum AlphaTestFunction : uint
    {
        Never = OpenGl.GL_NEVER,
        Less = OpenGl.GL_LESS,
        Equal = OpenGl.GL_EQUAL,
        LessThanOrEqual = OpenGl.GL_LEQUAL,
        Great = OpenGl.GL_GREATER,
        NotEqual = OpenGl.GL_NOTEQUAL,
        GreaterThanOrEqual = OpenGl.GL_GEQUAL,
        Always = OpenGl.GL_ALWAYS,
    }

    /// <summary>
    /// The OpenGl Attribute flags.
    /// </summary>
    [Flags]
    public enum AttributeMask : uint
    {
        None = 0,
        Current = OpenGl.GL_CURRENT_BIT,
        Point = OpenGl.GL_POINT_BIT,
        Line = OpenGl.GL_LINE_BIT,
        Polygon = OpenGl.GL_POLYGON_BIT,
        PolygonStipple = OpenGl.GL_POLYGON_STIPPLE_BIT,
        PixelMode = OpenGl.GL_PIXEL_MODE_BIT,
        Lighting = OpenGl.GL_LIGHTING_BIT,
        Fog = OpenGl.GL_FOG_BIT,
        DepthBuffer = OpenGl.GL_DEPTH_BUFFER_BIT,
        AccumBuffer = OpenGl.GL_ACCUM_BUFFER_BIT,
        StencilBuffer = OpenGl.GL_STENCIL_BUFFER_BIT,
        Viewport = OpenGl.GL_VIEWPORT_BIT,
        Transform = OpenGl.GL_TRANSFORM_BIT,
        Enable = OpenGl.GL_ENABLE_BIT,
        ColorBuffer = OpenGl.GL_COLOR_BUFFER_BIT,
        Hint = OpenGl.GL_HINT_BIT,
        Eval = OpenGl.GL_EVAL_BIT,
        List = OpenGl.GL_LIST_BIT,
        Texture = OpenGl.GL_TEXTURE_BIT,
        Scissor = OpenGl.GL_SCISSOR_BIT,
        All = OpenGl.GL_ALL_ATTRIB_BITS,
    }

    /// <summary>
    /// The begin mode.
    /// </summary>
    public enum BeginMode : uint
    {
        Points = OpenGl.GL_POINTS,
        Lines = OpenGl.GL_LINES,
        LineLoop = OpenGl.GL_LINE_LOOP,
        LineStrip = OpenGl.GL_LINE_STRIP,
        Triangles = OpenGl.GL_TRIANGLES,
        TriangleString = OpenGl.GL_TRIANGLE_STRIP,
        TriangleFan = OpenGl.GL_TRIANGLE_FAN,
        Quads = OpenGl.GL_QUADS,
        QuadStrip = OpenGl.GL_QUAD_STRIP,
        Polygon = OpenGl.GL_POLYGON
    }

    /// <summary>
    /// BlendingDestinationFactor
    /// </summary>
    public enum BlendingDestinationFactor : uint
    {
        Zero = OpenGl.GL_ZERO,
        One = OpenGl.GL_ONE,
        SourceColor = OpenGl.GL_SRC_COLOR,
        OneMinusSourceColor = OpenGl.GL_ONE_MINUS_SRC_COLOR,
        SourceAlpha = OpenGl.GL_SRC_ALPHA,
        OneMinusSourceAlpha = OpenGl.GL_ONE_MINUS_SRC_ALPHA,
        DestinationAlpha = OpenGl.GL_DST_ALPHA,
        OneMinusDestinationAlpha = OpenGl.GL_ONE_MINUS_DST_ALPHA,
    }

    /// <summary>
    /// The blending source factor.
    /// </summary>
    public enum BlendingSourceFactor : uint
    {
        DestinationColor = OpenGl.GL_DST_COLOR,
        OneMinusDestinationColor = OpenGl.GL_ONE_MINUS_DST_COLOR,
        SourceAlphaSaturate = OpenGl.GL_SRC_ALPHA_SATURATE,
        /// <summary>
        /// 
        /// </summary>
        SourceAlpha = OpenGl.GL_SRC_ALPHA
    }

    /// <summary>
    /// The Clip Plane Name
    /// </summary>
    public enum ClipPlaneName : uint
    {
        ClipPlane0 = OpenGl.GL_CLIP_PLANE0,
        ClipPlane1 = OpenGl.GL_CLIP_PLANE1,
        ClipPlane2 = OpenGl.GL_CLIP_PLANE2,
        ClipPlane3 = OpenGl.GL_CLIP_PLANE3,
        ClipPlane4 = OpenGl.GL_CLIP_PLANE4,
        ClipPlane5 = OpenGl.GL_CLIP_PLANE5
    }

    /// <summary>
    /// The Cull Face mode.
    /// </summary>
    public enum FaceMode : uint
    {
        /// <summary>
        /// 
        /// </summary>
        Front = OpenGl.GL_FRONT,
        FrontAndBack = OpenGl.GL_FRONT_AND_BACK,
        Back = OpenGl.GL_BACK,
    }

    /// <summary>
    /// The Data Type.
    /// </summary>
    public enum DataType : uint
    {
        Byte = OpenGl.GL_BYTE,
        UnsignedByte = OpenGl.GL_UNSIGNED_BYTE,
        Short = OpenGl.GL_SHORT,
        UnsignedShort = OpenGl.GL_UNSIGNED_SHORT,
        Int = OpenGl.GL_INT,
        UnsignedInt = OpenGl.GL_UNSIGNED_INT,
        Float = OpenGl.GL_FLOAT,
        TwoBytes = OpenGl.GL_2_BYTES,
        ThreeBytes = OpenGl.GL_3_BYTES,
        FourBytes = OpenGl.GL_4_BYTES,
        /// <summary>
        /// 
        /// </summary>
        Double = OpenGl.GL_DOUBLE
    }

    /// <summary>
    /// The depth function
    /// </summary>
    public enum DepthFunction : uint
    {
        Never = OpenGl.GL_NEVER,
        Less = OpenGl.GL_LESS,
        Equal = OpenGl.GL_EQUAL,
        LessThanOrEqual = OpenGl.GL_LEQUAL,
        Great = OpenGl.GL_GREATER,
        NotEqual = OpenGl.GL_NOTEQUAL,
        GreaterThanOrEqual = OpenGl.GL_GEQUAL,
        Always = OpenGl.GL_ALWAYS,
    }

    /// <summary>
    /// The Draw Buffer Mode
    /// </summary>
    public enum DrawBufferMode : uint
    {
        None = OpenGl.GL_NONE,
        FrontLeft = OpenGl.GL_FRONT_LEFT,
        FrontRight = OpenGl.GL_FRONT_RIGHT,
        BackLeft = OpenGl.GL_BACK_LEFT,
        BackRight = OpenGl.GL_BACK_RIGHT,
        Front = OpenGl.GL_FRONT,
        Back = OpenGl.GL_BACK,
        Left = OpenGl.GL_LEFT,
        Right = OpenGl.GL_RIGHT,
        FrontAndBack = OpenGl.GL_FRONT_AND_BACK,
        Auxilliary0 = OpenGl.GL_AUX0,
        Auxilliary1 = OpenGl.GL_AUX1,
        Auxilliary2 = OpenGl.GL_AUX2,
        Auxilliary3 = OpenGl.GL_AUX3,
    }

    /// <summary>
    /// Error Code
    /// </summary>
    public enum ErrorCode : uint
    {
        NoError = OpenGl.GL_NO_ERROR,
        InvalidEnum = OpenGl.GL_INVALID_ENUM,
        InvalidValue = OpenGl.GL_INVALID_VALUE,
        InvalidOperation = OpenGl.GL_INVALID_OPERATION,
        StackOverflow = OpenGl.GL_STACK_OVERFLOW,
        StackUnderflow = OpenGl.GL_STACK_UNDERFLOW,
        OutOfMemory = OpenGl.GL_OUT_OF_MEMORY
    }

    /// <summary>
    /// FeedBackMode
    /// </summary>
    public enum FeedbackMode : uint
    {
        TwoD = OpenGl.GL_2D,
        ThreeD = OpenGl.GL_3D,
        FourD = OpenGl.GL_4D_COLOR,
        ThreeDColorTexture = OpenGl.GL_3D_COLOR_TEXTURE,
        FourDColorTexture = OpenGl.GL_4D_COLOR_TEXTURE
    }

    /// <summary>
    /// The Feedback Token
    /// </summary>
    public enum FeedbackToken : uint
    {
        PassThroughToken = OpenGl.GL_PASS_THROUGH_TOKEN,
        PointToken = OpenGl.GL_POINT_TOKEN,
        LineToken = OpenGl.GL_LINE_TOKEN,
        PolygonToken = OpenGl.GL_POLYGON_TOKEN,
        BitmapToken = OpenGl.GL_BITMAP_TOKEN,
        DrawPixelToken = OpenGl.GL_DRAW_PIXEL_TOKEN,
        CopyPixelToken = OpenGl.GL_COPY_PIXEL_TOKEN,
        LineResetToken = OpenGl.GL_LINE_RESET_TOKEN
    }

    /// <summary>
    /// The Fog Mode.
    /// </summary>
    public enum FogMode : uint
    {
        Exp = OpenGl.GL_EXP,

        /// <summary>
        /// 
        /// </summary>
		Exp2 = OpenGl.GL_EXP2,
    }

    /// <summary>
    /// GetMapTarget 
    /// </summary>
    public enum GetMapTarget : uint
    {
        Coeff = OpenGl.GL_COEFF,
        Order = OpenGl.GL_ORDER,
        Domain = OpenGl.GL_DOMAIN
    }

    public enum GetTarget : uint
    {
        CurrentColor = OpenGl.GL_CURRENT_COLOR,
        CurrentIndex = OpenGl.GL_CURRENT_INDEX,
        CurrentNormal = OpenGl.GL_CURRENT_NORMAL,
        CurrentTextureCoords = OpenGl.GL_CURRENT_TEXTURE_COORDS,
        CurrentRasterColor = OpenGl.GL_CURRENT_RASTER_COLOR,
        CurrentRasterIndex = OpenGl.GL_CURRENT_RASTER_INDEX,
        CurrentRasterTextureCoords = OpenGl.GL_CURRENT_RASTER_TEXTURE_COORDS,
        CurrentRasterPosition = OpenGl.GL_CURRENT_RASTER_POSITION,
        CurrentRasterPositionValid = OpenGl.GL_CURRENT_RASTER_POSITION_VALID,
        CurrentRasterDistance = OpenGl.GL_CURRENT_RASTER_DISTANCE,
        PointSmooth = OpenGl.GL_POINT_SMOOTH,
        PointSize = OpenGl.GL_POINT_SIZE,
        PointSizeRange = OpenGl.GL_POINT_SIZE_RANGE,
        PointSizeGranularity = OpenGl.GL_POINT_SIZE_GRANULARITY,
        LineSmooth = OpenGl.GL_LINE_SMOOTH,
        LineWidth = OpenGl.GL_LINE_WIDTH,
        LineWidthRange = OpenGl.GL_LINE_WIDTH_RANGE,
        LineWidthGranularity = OpenGl.GL_LINE_WIDTH_GRANULARITY,
        LineStipple = OpenGl.GL_LINE_STIPPLE,
        LineStipplePattern = OpenGl.GL_LINE_STIPPLE_PATTERN,
        LineStippleRepeat = OpenGl.GL_LINE_STIPPLE_REPEAT,
        ListMode = OpenGl.GL_LIST_MODE,
        MaxListNesting = OpenGl.GL_MAX_LIST_NESTING,
        ListBase = OpenGl.GL_LIST_BASE,
        ListIndex = OpenGl.GL_LIST_INDEX,
        PolygonMode = OpenGl.GL_POLYGON_MODE,
        PolygonSmooth = OpenGl.GL_POLYGON_SMOOTH,
        PolygonStipple = OpenGl.GL_POLYGON_STIPPLE,
        EdgeFlag = OpenGl.GL_EDGE_FLAG,
        CullFace = OpenGl.GL_CULL_FACE,
        CullFaceMode = OpenGl.GL_CULL_FACE_MODE,
        FrontFace = OpenGl.GL_FRONT_FACE,
        Lighting = OpenGl.GL_LIGHTING,
        LightModelLocalViewer = OpenGl.GL_LIGHT_MODEL_LOCAL_VIEWER,
        LightModelTwoSide = OpenGl.GL_LIGHT_MODEL_TWO_SIDE,
        LightModelAmbient = OpenGl.GL_LIGHT_MODEL_AMBIENT,
        ShadeModel = OpenGl.GL_SHADE_MODEL,
        ColorMaterialFace = OpenGl.GL_COLOR_MATERIAL_FACE,
        ColorMaterialParameter = OpenGl.GL_COLOR_MATERIAL_PARAMETER,
        ColorMaterial = OpenGl.GL_COLOR_MATERIAL,
        Fog = OpenGl.GL_FOG,
        FogIndex = OpenGl.GL_FOG_INDEX,
        FogDensity = OpenGl.GL_FOG_DENSITY,
        FogStart = OpenGl.GL_FOG_START,
        FogEnd = OpenGl.GL_FOG_END,
        FogMode = OpenGl.GL_FOG_MODE,
        FogColor = OpenGl.GL_FOG_COLOR,
        DepthRange = OpenGl.GL_DEPTH_RANGE,
        DepthTest = OpenGl.GL_DEPTH_TEST,
        DepthWritemask = OpenGl.GL_DEPTH_WRITEMASK,
        DepthClearValue = OpenGl.GL_DEPTH_CLEAR_VALUE,
        DepthFunc = OpenGl.GL_DEPTH_FUNC,
        AccumClearValue = OpenGl.GL_ACCUM_CLEAR_VALUE,
        StencilTest = OpenGl.GL_STENCIL_TEST,
        StencilClearValue = OpenGl.GL_STENCIL_CLEAR_VALUE,
        StencilFunc = OpenGl.GL_STENCIL_FUNC,
        StencilValueMask = OpenGl.GL_STENCIL_VALUE_MASK,
        StencilFail = OpenGl.GL_STENCIL_FAIL,
        StencilPassDepthFail = OpenGl.GL_STENCIL_PASS_DEPTH_FAIL,
        StencilPassDepthPass = OpenGl.GL_STENCIL_PASS_DEPTH_PASS,
        StencilRef = OpenGl.GL_STENCIL_REF,
        StencilWritemask = OpenGl.GL_STENCIL_WRITEMASK,
        MatrixMode = OpenGl.GL_MATRIX_MODE,
        Normalize = OpenGl.GL_NORMALIZE,
        Viewport = OpenGl.GL_VIEWPORT,
        ModelviewStackDepth = OpenGl.GL_MODELVIEW_STACK_DEPTH,
        ProjectionStackDepth = OpenGl.GL_PROJECTION_STACK_DEPTH,
        TextureStackDepth = OpenGl.GL_TEXTURE_STACK_DEPTH,
        ModelviewMatix = OpenGl.GL_MODELVIEW_MATRIX,
        ProjectionMatrix = OpenGl.GL_PROJECTION_MATRIX,
        TextureMatrix = OpenGl.GL_TEXTURE_MATRIX,
        AttribStackDepth = OpenGl.GL_ATTRIB_STACK_DEPTH,
        ClientAttribStackDepth = OpenGl.GL_CLIENT_ATTRIB_STACK_DEPTH,
        AlphaTest = OpenGl.GL_ALPHA_TEST,
        AlphaTestFunc = OpenGl.GL_ALPHA_TEST_FUNC,
        AlphaTestRef = OpenGl.GL_ALPHA_TEST_REF,
        Dither = OpenGl.GL_DITHER,
        BlendDst = OpenGl.GL_BLEND_DST,
        BlendSrc = OpenGl.GL_BLEND_SRC,
        Blend = OpenGl.GL_BLEND,
        LogicOpMode = OpenGl.GL_LOGIC_OP_MODE,
        IndexLogicOp = OpenGl.GL_INDEX_LOGIC_OP,
        ColorLogicOp = OpenGl.GL_COLOR_LOGIC_OP,
        AuxBuffers = OpenGl.GL_AUX_BUFFERS,
        DrawBuffer = OpenGl.GL_DRAW_BUFFER,
        ReadBuffer = OpenGl.GL_READ_BUFFER,
        ScissorBox = OpenGl.GL_SCISSOR_BOX,
        ScissorTest = OpenGl.GL_SCISSOR_TEST,
        IndexClearValue = OpenGl.GL_INDEX_CLEAR_VALUE,
        IndexWritemask = OpenGl.GL_INDEX_WRITEMASK,
        ColorClearValue = OpenGl.GL_COLOR_CLEAR_VALUE,
        ColorWritemask = OpenGl.GL_COLOR_WRITEMASK,
        IndexMode = OpenGl.GL_INDEX_MODE,
        RgbaMode = OpenGl.GL_RGBA_MODE,
        DoubleBuffer = OpenGl.GL_DOUBLEBUFFER,
        Stereo = OpenGl.GL_STEREO,
        RenderMode = OpenGl.GL_RENDER_MODE,
        PerspectiveCorrectionHint = OpenGl.GL_PERSPECTIVE_CORRECTION_HINT,
        PointSmoothHint = OpenGl.GL_POINT_SMOOTH_HINT,
        LineSmoothHint = OpenGl.GL_LINE_SMOOTH_HINT,
        PolygonSmoothHint = OpenGl.GL_POLYGON_SMOOTH_HINT,
        FogHint = OpenGl.GL_FOG_HINT,
        TextureGenS = OpenGl.GL_TEXTURE_GEN_S,
        TextureGenT = OpenGl.GL_TEXTURE_GEN_T,
        TextureGenR = OpenGl.GL_TEXTURE_GEN_R,
        TextureGenQ = OpenGl.GL_TEXTURE_GEN_Q,
        PixelMapItoI = OpenGl.GL_PIXEL_MAP_I_TO_I,
        PixelMapStoS = OpenGl.GL_PIXEL_MAP_S_TO_S,
        PixelMapItoR = OpenGl.GL_PIXEL_MAP_I_TO_R,
        PixelMapItoG = OpenGl.GL_PIXEL_MAP_I_TO_G,
        PixelMapItoB = OpenGl.GL_PIXEL_MAP_I_TO_B,
        PixelMapItoA = OpenGl.GL_PIXEL_MAP_I_TO_A,
        PixelMapRtoR = OpenGl.GL_PIXEL_MAP_R_TO_R,
        PixelMapGtoG = OpenGl.GL_PIXEL_MAP_G_TO_G,
        PixelMapBtoB = OpenGl.GL_PIXEL_MAP_B_TO_B,
        PixelMapAtoA = OpenGl.GL_PIXEL_MAP_A_TO_A,
        PixelMapItoISize = OpenGl.GL_PIXEL_MAP_I_TO_I_SIZE,
        PixelMapStoSSize = OpenGl.GL_PIXEL_MAP_S_TO_S_SIZE,
        PixelMapItoRSize = OpenGl.GL_PIXEL_MAP_I_TO_R_SIZE,
        PixelMapItoGSize = OpenGl.GL_PIXEL_MAP_I_TO_G_SIZE,
        PixelMapItoBSize = OpenGl.GL_PIXEL_MAP_I_TO_B_SIZE,
        PixelMapItoASize = OpenGl.GL_PIXEL_MAP_I_TO_A_SIZE,
        PixelMapRtoRSize = OpenGl.GL_PIXEL_MAP_R_TO_R_SIZE,
        PixelMapGtoGSize = OpenGl.GL_PIXEL_MAP_G_TO_G_SIZE,
        PixelMapBtoBSize = OpenGl.GL_PIXEL_MAP_B_TO_B_SIZE,
        PixelMapAtoASize = OpenGl.GL_PIXEL_MAP_A_TO_A_SIZE,
        UnpackSwapBytes = OpenGl.GL_UNPACK_SWAP_BYTES,
        LsbFirst = OpenGl.GL_UNPACK_LSB_FIRST,
        UnpackRowLength = OpenGl.GL_UNPACK_ROW_LENGTH,
        UnpackSkipRows = OpenGl.GL_UNPACK_SKIP_ROWS,
        UnpackSkipPixels = OpenGl.GL_UNPACK_SKIP_PIXELS,
        UnpackAlignment = OpenGl.GL_UNPACK_ALIGNMENT,
        PackSwapBytes = OpenGl.GL_PACK_SWAP_BYTES,
        PackLsbFirst = OpenGl.GL_PACK_LSB_FIRST,
        PackRowLength = OpenGl.GL_PACK_ROW_LENGTH,
        PackSkipRows = OpenGl.GL_PACK_SKIP_ROWS,
        PackSkipPixels = OpenGl.GL_PACK_SKIP_PIXELS,
        PackAlignment = OpenGl.GL_PACK_ALIGNMENT,
        MapColor = OpenGl.GL_MAP_COLOR,
        MapStencil = OpenGl.GL_MAP_STENCIL,
        IndexShift = OpenGl.GL_INDEX_SHIFT,
        IndexOffset = OpenGl.GL_INDEX_OFFSET,
        RedScale = OpenGl.GL_RED_SCALE,
        RedBias = OpenGl.GL_RED_BIAS,
        ZoomX = OpenGl.GL_ZOOM_X,
        ZoomY = OpenGl.GL_ZOOM_Y,
        GreenScale = OpenGl.GL_GREEN_SCALE,
        GreenBias = OpenGl.GL_GREEN_BIAS,
        BlueScale = OpenGl.GL_BLUE_SCALE,
        BlueBias = OpenGl.GL_BLUE_BIAS,
        AlphaScale = OpenGl.GL_ALPHA_SCALE,
        AlphaBias = OpenGl.GL_ALPHA_BIAS,
        DepthScale = OpenGl.GL_DEPTH_SCALE,
        DepthBias = OpenGl.GL_DEPTH_BIAS,
        MapEvalOrder = OpenGl.GL_MAX_EVAL_ORDER,
        MaxLights = OpenGl.GL_MAX_LIGHTS,
        MaxClipPlanes = OpenGl.GL_MAX_CLIP_PLANES,
        MaxTextureSize = OpenGl.GL_MAX_TEXTURE_SIZE,
        MapPixelMapTable = OpenGl.GL_MAX_PIXEL_MAP_TABLE,
        MaxAttribStackDepth = OpenGl.GL_MAX_ATTRIB_STACK_DEPTH,
        MaxModelviewStackDepth = OpenGl.GL_MAX_MODELVIEW_STACK_DEPTH,
        MaxNameStackDepth = OpenGl.GL_MAX_NAME_STACK_DEPTH,
        MaxProjectionStackDepth = OpenGl.GL_MAX_PROJECTION_STACK_DEPTH,
        MaxTextureStackDepth = OpenGl.GL_MAX_TEXTURE_STACK_DEPTH,
        MaxViewportDims = OpenGl.GL_MAX_VIEWPORT_DIMS,
        MaxClientAttribStackDepth = OpenGl.GL_MAX_CLIENT_ATTRIB_STACK_DEPTH,
        SubpixelBits = OpenGl.GL_SUBPIXEL_BITS,
        IndexBits = OpenGl.GL_INDEX_BITS,
        RedBits = OpenGl.GL_RED_BITS,
        GreenBits = OpenGl.GL_GREEN_BITS,
        BlueBits = OpenGl.GL_BLUE_BITS,
        AlphaBits = OpenGl.GL_ALPHA_BITS,
        DepthBits = OpenGl.GL_DEPTH_BITS,
        StencilBits = OpenGl.GL_STENCIL_BITS,
        AccumRedBits = OpenGl.GL_ACCUM_RED_BITS,
        AccumGreenBits = OpenGl.GL_ACCUM_GREEN_BITS,
        AccumBlueBits = OpenGl.GL_ACCUM_BLUE_BITS,
        AccumAlphaBits = OpenGl.GL_ACCUM_ALPHA_BITS,
        NameStackDepth = OpenGl.GL_NAME_STACK_DEPTH,
        AutoNormal = OpenGl.GL_AUTO_NORMAL,
        Map1Color4 = OpenGl.GL_MAP1_COLOR_4,
        Map1Index = OpenGl.GL_MAP1_INDEX,
        Map1Normal = OpenGl.GL_MAP1_NORMAL,
        Map1TextureCoord1 = OpenGl.GL_MAP1_TEXTURE_COORD_1,
        Map1TextureCoord2 = OpenGl.GL_MAP1_TEXTURE_COORD_2,
        Map1TextureCoord3 = OpenGl.GL_MAP1_TEXTURE_COORD_3,
        Map1TextureCoord4 = OpenGl.GL_MAP1_TEXTURE_COORD_4,
        Map1Vertex3 = OpenGl.GL_MAP1_VERTEX_3,
        Map1Vertex4 = OpenGl.GL_MAP1_VERTEX_4,
        Map2Color4 = OpenGl.GL_MAP2_COLOR_4,
        Map2Index = OpenGl.GL_MAP2_INDEX,
        Map2Normal = OpenGl.GL_MAP2_NORMAL,
        Map2TextureCoord1 = OpenGl.GL_MAP2_TEXTURE_COORD_1,
        Map2TextureCoord2 = OpenGl.GL_MAP2_TEXTURE_COORD_2,
        Map2TextureCoord3 = OpenGl.GL_MAP2_TEXTURE_COORD_3,
        Map2TextureCoord4 = OpenGl.GL_MAP2_TEXTURE_COORD_4,
        Map2Vertex3 = OpenGl.GL_MAP2_VERTEX_3,
        Map2Vertex4 = OpenGl.GL_MAP2_VERTEX_4,
        Map1GridDomain = OpenGl.GL_MAP1_GRID_DOMAIN,
        Map1GridSegments = OpenGl.GL_MAP1_GRID_SEGMENTS,
        Map2GridDomain = OpenGl.GL_MAP2_GRID_DOMAIN,
        Map2GridSegments = OpenGl.GL_MAP2_GRID_SEGMENTS,
        Texture1D = OpenGl.GL_TEXTURE_1D,
        Texture2D = OpenGl.GL_TEXTURE_2D,
        FeedbackBufferPointer = OpenGl.GL_FEEDBACK_BUFFER_POINTER,
        FeedbackBufferSize = OpenGl.GL_FEEDBACK_BUFFER_SIZE,
        FeedbackBufferType = OpenGl.GL_FEEDBACK_BUFFER_TYPE,
        SelectionBufferPointer = OpenGl.GL_SELECTION_BUFFER_POINTER,
        SelectionBufferSize = OpenGl.GL_SELECTION_BUFFER_SIZE
    }

    /// <summary>
    /// The Front Face Mode.
    /// </summary>
    public enum FrontFaceMode : uint
    {
        ClockWise = OpenGl.GL_CW,
        CounterClockWise = OpenGl.GL_CCW,
    }


    /// <summary>
    /// The hint mode.
    /// </summary>
	public enum HintMode : uint
    {
        DontCare = OpenGl.GL_DONT_CARE,
        Fastest = OpenGl.GL_FASTEST,
        /// <summary>
        /// The 
        /// </summary>
        Nicest = OpenGl.GL_NICEST
    }

    /// <summary>
    /// The hint target.
    /// </summary>
    public enum HintTarget : uint
    {
        PerspectiveCorrection = OpenGl.GL_PERSPECTIVE_CORRECTION_HINT,
        PointSmooth = OpenGl.GL_POINT_SMOOTH_HINT,
        LineSmooth = OpenGl.GL_LINE_SMOOTH_HINT,
        PolygonSmooth = OpenGl.GL_POLYGON_SMOOTH_HINT,
        Fog = OpenGl.GL_FOG_HINT
    }

    /// <summary>
    /// LightName
    /// </summary>
    public enum LightName : uint
    {
        Light0 = OpenGl.GL_LIGHT0,
        Light1 = OpenGl.GL_LIGHT1,
        Light2 = OpenGl.GL_LIGHT2,
        Light3 = OpenGl.GL_LIGHT3,
        Light4 = OpenGl.GL_LIGHT4,
        Light5 = OpenGl.GL_LIGHT5,
        Light6 = OpenGl.GL_LIGHT6,
        Light7 = OpenGl.GL_LIGHT7
    }

    /// <summary>
    /// LightParameter
    /// </summary>
    public enum LightParameter : uint
    {
        Ambient = OpenGl.GL_AMBIENT,
        Diffuse = OpenGl.GL_DIFFUSE,
        Specular = OpenGl.GL_SPECULAR,
        Position = OpenGl.GL_POSITION,
        SpotDirection = OpenGl.GL_SPOT_DIRECTION,
        SpotExponent = OpenGl.GL_SPOT_EXPONENT,
        SpotCutoff = OpenGl.GL_SPOT_CUTOFF,
        ConstantAttenuatio = OpenGl.GL_CONSTANT_ATTENUATION,
        LinearAttenuation = OpenGl.GL_LINEAR_ATTENUATION,
        QuadraticAttenuation = OpenGl.GL_QUADRATIC_ATTENUATION
    }

    /// <summary>
    /// The Light Model Parameter.
    /// </summary>
    public enum LightModelParameter : uint
    {
        LocalViewer = OpenGl.GL_LIGHT_MODEL_LOCAL_VIEWER,
        TwoSide = OpenGl.GL_LIGHT_MODEL_TWO_SIDE,
        Ambient = OpenGl.GL_LIGHT_MODEL_AMBIENT
    }

    /// <summary>
    /// The Logic Op
    /// </summary>
    public enum LogicOp : uint
    {
        Clear = OpenGl.GL_CLEAR,
        And = OpenGl.GL_AND,
        AndReverse = OpenGl.GL_AND_REVERSE,
        Copy = OpenGl.GL_COPY,
        AndInverted = OpenGl.GL_AND_INVERTED,
        NoOp = OpenGl.GL_NOOP,
        XOr = OpenGl.GL_XOR,
        Or = OpenGl.GL_OR,
        NOr = OpenGl.GL_NOR,
        Equiv = OpenGl.GL_EQUIV,
        Invert = OpenGl.GL_INVERT,
        OrReverse = OpenGl.GL_OR_REVERSE,
        CopyInverted = OpenGl.GL_COPY_INVERTED,
        OrInverted = OpenGl.GL_OR_INVERTED,
        NAnd = OpenGl.GL_NAND,
        Set = OpenGl.GL_SET,
    }

    /// <summary>
    /// The matrix mode.
    /// </summary>
    public enum MatrixMode : uint
    {
        Modelview = OpenGl.GL_MODELVIEW,
        Projection = OpenGl.GL_PROJECTION,
        Texture = OpenGl.GL_TEXTURE
    }

    /// <summary>
    /// The pixel transfer parameter name
    /// </summary>
    public enum PixelTransferParameterName : uint
    {
        MapColor = OpenGl.GL_MAP_COLOR,
        MapStencil = OpenGl.GL_MAP_STENCIL,
        IndexShift = OpenGl.GL_INDEX_SHIFT,
        IndexOffset = OpenGl.GL_INDEX_OFFSET,
        RedScale = OpenGl.GL_RED_SCALE,
        RedBias = OpenGl.GL_RED_BIAS,
        ZoomX = OpenGl.GL_ZOOM_X,
        ZoomY = OpenGl.GL_ZOOM_Y,
        GreenScale = OpenGl.GL_GREEN_SCALE,
        GreenBias = OpenGl.GL_GREEN_BIAS,
        BlueScale = OpenGl.GL_BLUE_SCALE,
        BlueBias = OpenGl.GL_BLUE_BIAS,
        AlphaScale = OpenGl.GL_ALPHA_SCALE,
        AlphaBias = OpenGl.GL_ALPHA_BIAS,
        DepthScale = OpenGl.GL_DEPTH_SCALE,
        DepthBias = OpenGl.GL_DEPTH_BIAS
    }

    /// <summary>
    /// The Polygon mode.
    /// </summary>
    public enum PolygonMode : uint
    {
        /// <summary>
        /// Render as points.
        /// </summary>
        Points = OpenGl.GL_POINT,

        /// <summary>
        /// Render as lines.
        /// </summary>
        Lines = OpenGl.GL_LINE,

        /// <summary>
        /// Render as filled.
        /// </summary>
        Filled = OpenGl.GL_FILL
    }

    /// <summary>
    /// Rendering Mode 
    /// </summary>
    public enum RenderingMode : uint
    {
        Render = OpenGl.GL_RENDER,
        Feedback = OpenGl.GL_FEEDBACK,
        Select = OpenGl.GL_SELECT
    }

    /// <summary>
    /// ShadingModel
    /// </summary>
	public enum ShadeModel : uint
    {
        Flat = OpenGl.GL_FLAT,
        Smooth = OpenGl.GL_SMOOTH
    }

    /// <summary>
    /// The stencil function
    /// </summary>
    public enum StencilFunction : uint
    {
        Never = OpenGl.GL_NEVER,
        Less = OpenGl.GL_LESS,
        Equal = OpenGl.GL_EQUAL,
        LessThanOrEqual = OpenGl.GL_LEQUAL,
        Great = OpenGl.GL_GREATER,
        NotEqual = OpenGl.GL_NOTEQUAL,
        GreaterThanOrEqual = OpenGl.GL_GEQUAL,
        Always = OpenGl.GL_ALWAYS,
    }

    /// <summary>
    /// The stencil operation.
    /// </summary>
    public enum StencilOperation : uint
    {
        Keep = OpenGl.GL_KEEP,
        Replace = OpenGl.GL_REPLACE,
        Increase = OpenGl.GL_INCR,
        Decrease = OpenGl.GL_DECR,
        Zero = OpenGl.GL_ZERO,
        //IncreaseWrap = OpenGl.GL_INCR_WRAP,
        //DecreaseWrap = OpenGl.GL_DECR_WRAP,
        Invert = OpenGl.GL_INVERT
    }

    /// <summary>
    /// GetTextureParameter
    /// </summary>
    public enum TextureParameter : uint
    {
        TextureWidth = OpenGl.GL_TEXTURE_WIDTH,
        TextureHeight = OpenGl.GL_TEXTURE_HEIGHT,
        TextureInternalFormat = OpenGl.GL_TEXTURE_INTERNAL_FORMAT,
        TextureBorderColor = OpenGl.GL_TEXTURE_BORDER_COLOR,
        TextureBorder = OpenGl.GL_TEXTURE_BORDER
    }

    /// <summary>
    /// Texture target.
    /// </summary>
    public enum TextureTarget : uint
    {
        Texture1D = OpenGl.GL_TEXTURE_1D,
        Texture2D = OpenGl.GL_TEXTURE_2D,
        //Texture3D = OpenGl.GL_TEXTURE_3D
    }

    /// <summary>
    /// The GLYPHMETRICSFLOAT structure contains information about the placement and orientation of a glyph in a character cell.
    /// </summary>
    public struct GLYPHMETRICSFLOAT
    {
        /// <summary>
        /// Specifies the width of the smallest rectangle (the glyph's black box) that completely encloses the glyph..
        /// </summary>
        public float gmfBlackBoxX;
        /// <summary>
        /// Specifies the height of the smallest rectangle (the glyph's black box) that completely encloses the glyph.
        /// </summary>
        public float gmfBlackBoxY;
        /// <summary>
        /// Specifies the x and y coordinates of the upper-left corner of the smallest rectangle that completely encloses the glyph.
        /// </summary>
        public POINTFLOAT gmfptGlyphOrigin;
        /// <summary>
        /// Specifies the horizontal distance from the origin of the current character cell to the origin of the next character cell.
        /// </summary>
        public float gmfCellIncX;
        /// <summary>
        /// Specifies the vertical distance from the origin of the current character cell to the origin of the next character cell.
        /// </summary>
        public float gmfCellIncY;
    }

    /// <summary>
    /// Point structure used in Win32 interop.
    /// </summary>
    public struct POINTFLOAT
    {
        /// <summary>
        /// The x coord value.
        /// </summary>
        public float x;

        /// <summary>
        /// The y coord value.
        /// </summary>
        public float y;

    }
}
