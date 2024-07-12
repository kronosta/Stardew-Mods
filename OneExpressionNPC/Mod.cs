namespace OneExpressionNPC
{
    internal sealed class ModEntry : StardewModdingAPI.Mod
    {
        public override void Entry(StardewModdingAPI.IModHelper helper)
        {
            ((System.Func<System.Collections.Generic.Dictionary<string, object>, object>)(vars => 
                new object[]
                {
                    true,
                    ((System.Func<System.Reflection.EventInfo, object>)(check1 =>
                        check1 == null
                        ?
                        ((System.Reflection.MethodInfo)vars["Method: Monitor.Log"])
                        .Invoke(Monitor, new object[]{ "AssetRequested event could not be found. ", StardewModdingAPI.LogLevel.Error})
                        :
                        new object[]
                        {
                           vars["Event: AssetRequested"] = check1,
                           vars["Function: Redify"] = ((System.Func<Microsoft.Xna.Framework.Graphics.Texture2D, Microsoft.Xna.Framework.Graphics.Texture2D>)(t =>
                               ((Microsoft.Xna.Framework.Graphics.Texture2D)System.Linq.Enumerable.Last(
                                   new object[] {
                                       vars["State: Texture to redify"] = t,
                                       StardewValley.Game1.content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>("Data/Kronosta/OneExpressionNPC/Redified")
                                   }
                               ))      
                           )),
                           typeof(System.Reflection.EventInfo)
                            .GetMethod("AddEventHandler")
                            .Invoke(helper.Events.Content, 
                                new object[] {
                                System.Linq.Expressions.Expression.Lambda<System.Action<object, StardewModdingAPI.Events.AssetRequestedEventArgs>>(
                                    (new dynamic[] {
                                    @"
                                    This condition checks if the asset being requested is Data/NPCDispositions or not.=
                                    If true: Add the disposition to it.
                                    If false: Go down the conditional tower, since System.Linq.Expressions.Expression doesn't have else if directly.
                                    ",
                                    System.Linq.Expressions.Expression.Condition(
                                        System.Linq.Expressions.Expression.Call(
                                            typeof(StardewModdingAPI.IAssetName)
                                                .GetMethod("IsEquivalentTo", new System.Type[]{typeof(string)}),
                                            System.Linq.Expressions.Expression.Property(
                                                ((System.Linq.Expressions.ParameterExpression)vars["LinqParam: e"]),
                                                "NameWithoutLocale"
                                            ),
                                            System.Linq.Expressions.Expression.Constant("Data/NPCDispositions")
                                        ),
                                        System.Linq.Expressions.Expression.Call(
                                            ((System.Linq.Expressions.ParameterExpression)vars["LinqParam: e"]),
                                            typeof(StardewModdingAPI.Events.AssetRequestedEventArgs)
                                                .GetMethod("Edit", new System.Type[]
                                                {
                                                    typeof(System.Action<StardewModdingAPI.IAssetData>),
                                                    typeof(StardewModdingAPI.Events.AssetEditPriority),
                                                    typeof(string)
                                                }),
                                            System.Linq.Expressions.Expression.Lambda<System.Action<StardewModdingAPI.IAssetData>>(
                                                System.Linq.Expressions.Expression.Assign(
                                                    System.Linq.Expressions.Expression.Property(
                                                        System.Linq.Expressions.Expression.Property(
                                                            System.Linq.Expressions.Expression.Call(
                                                                ((System.Linq.Expressions.ParameterExpression)vars["LinqParam: asset 1"]),
                                                                typeof(StardewModdingAPI.IAssetData)
                                                                    .GetMethod("AsDictionary", System.Type.EmptyTypes)
                                                                    .MakeGenericMethod(typeof(string), typeof(string))
                                                            ),
                                                            "Data"
                                                        ),
                                                        typeof(StardewModdingAPI.IAssetDataForDictionary<,>)
                                                            .MakeGenericType(typeof(string), typeof(string))
                                                            .GetProperty("Item", new System.Type[]{typeof(string)}),
                                                        System.Linq.Expressions.Expression.Constant("Dobson")
                                                    ),
                                                    (new dynamic[] {
                                                        System.Linq.Expressions.Expression.Constant("adult/rude/neutral/positive/male/not-datable//Town/summer 7//BusStop 19 4/Dobson"),
                                                        @"
                                                        After all of this crazy amount of boilerplate, we finally have added the NPC to the game.

                                                        This is also a nice comment style if you're being harder on yourself in this challenge
                                                        by banning everything except for the namespace, class, and Entry method declarations and the one statement.
                                                        This ultra-hard version bans imports and comments, and that's what I'm doing, so
                                                        that's why everything is fully qualified.

                                                        The cool thing about this comment style is that is can be placed anywhere in an expression where is is standalone.
                                                        A few examples:
                                                            (new dynamic[] { 5, ""Funny that this is a prime number when we use it so often."" [0]) * 17},
                                                            Console.WriteLine((new dynamic[] { ""Hello, world!"", ""My first ever string""}[0]))
                                                            (new dynamic[] {(new dynamic[] { Console.ReadKey(), ""Read in a console key."" }[0]).KeyChar, ""The key.""}[0]) == 'j'
                                                        "
                                                    }[0])
                                                ),
                                                false,
                                                new System.Linq.Expressions.ParameterExpression[]
                                                {
                                                    ((System.Linq.Expressions.ParameterExpression)vars["LinqParam: asset 1"])
                                                }
                                            ),
                                            System.Linq.Expressions.Expression.Constant(StardewModdingAPI.Events.AssetEditPriority.Default),
                                            System.Linq.Expressions.Expression.Constant(null)
                                        ),
                                        (new dynamic[] {
                                        @"
                                        This condition checks if the asset being requested is Data/NPCGiftTastes.
                                        If true: add the gift tastes.
                                        If false: continue down the chain.
                                        ",
                                        System.Linq.Expressions.Expression.Condition(
                                            System.Linq.Expressions.Expression.Call(
                                                typeof(StardewModdingAPI.IAssetName)
                                                    .GetMethod("IsEquivalentTo", new System.Type[]{typeof(string)}),
                                                System.Linq.Expressions.Expression.Property(
                                                    ((System.Linq.Expressions.ParameterExpression)vars["LinqParam: e"]),
                                                    "NameWithoutLocale"
                                                ),
                                                System.Linq.Expressions.Expression.Constant("Data/NPCGiftTastes")
                                            ),
                                            System.Linq.Expressions.Expression.Call(
                                                ((System.Linq.Expressions.ParameterExpression)vars["LinqParam: e"]),
                                                typeof(StardewModdingAPI.Events.AssetRequestedEventArgs)
                                                    .GetMethod("Edit", new System.Type[]
                                                    {
                                                        typeof(System.Action<StardewModdingAPI.IAssetData>),
                                                        typeof(StardewModdingAPI.Events.AssetEditPriority),
                                                        typeof(string)
                                                    }),
                                                System.Linq.Expressions.Expression.Lambda<System.Action<StardewModdingAPI.IAssetData>>(
                                                    System.Linq.Expressions.Expression.Assign(
                                                        System.Linq.Expressions.Expression.Property(
                                                            System.Linq.Expressions.Expression.Property(
                                                                System.Linq.Expressions.Expression.Call(
                                                                    ((System.Linq.Expressions.ParameterExpression)vars["LinqParam: asset 2"]),
                                                                    typeof(StardewModdingAPI.IAssetData)
                                                                        .GetMethod("AsDictionary", System.Type.EmptyTypes)
                                                                        .MakeGenericMethod(typeof(string), typeof(string))
                                                                ),
                                                                "Data"
                                                            ),
                                                            typeof(StardewModdingAPI.IAssetDataForDictionary<,>)
                                                                .MakeGenericType(typeof(string), typeof(string))
                                                                .GetProperty("Item", new System.Type[]{typeof(string)}),
                                                            System.Linq.Expressions.Expression.Constant("Dobson")
                                                        ),
                                                        System.Linq.Expressions.Expression.Constant("You're giving this to me? This is amazing!/207 232 233 400/Thank you! This is a very interesting specimen./-5 -79 422/...What is this?/80 330/This is disgusting./2/That was very thoughtful of you./-4/ ")
                                                    ),
                                                    false,
                                                    new System.Linq.Expressions.ParameterExpression[]
                                                    {
                                                        ((System.Linq.Expressions.ParameterExpression)vars["LinqParam: asset 2"]),
                                                    }
                                                ),
                                                System.Linq.Expressions.Expression.Constant(StardewModdingAPI.Events.AssetEditPriority.Default),
                                                System.Linq.Expressions.Expression.Constant(null)
                                            ),
                                            (new dynamic[] {
                                            @"
                                            This conditional checks if the asset being requested is Characters/Dobson.
                                            If true: add the sprites.
                                            If false: continue down the tower.
                                            ",
                                            System.Linq.Expressions.Expression.Condition(
                                                System.Linq.Expressions.Expression.Call(
                                                    typeof(StardewModdingAPI.IAssetName)
                                                        .GetMethod("IsEquivalentTo", new System.Type[]{typeof(string)}),
                                                    System.Linq.Expressions.Expression.Property(
                                                        ((System.Linq.Expressions.ParameterExpression)vars["LinqParam: e"]),
                                                        "NameWithoutLocale"
                                                    ),
                                                    System.Linq.Expressions.Expression.Constant("Characters/Dobson")
                                                ),
                                                System.Linq.Expressions.Expression.Call(
                                                    ((System.Linq.Expressions.ParameterExpression)vars["LinqParam: e"]),
                                                    typeof(StardewModdingAPI.Events.AssetRequestedEventArgs)
                                                        .GetMethod(
                                                            "LoadFrom",
                                                            new System.Type[] {
                                                                typeof(System.Func<object>),
                                                                typeof(StardewModdingAPI.Events.AssetLoadPriority),
                                                                typeof(string)
                                                            }
                                                        ),
                                                    System.Linq.Expressions.Expression.Constant(((System.Func<object>)(() =>
                                                        ((System.Func<Microsoft.Xna.Framework.Graphics.Texture2D, Microsoft.Xna.Framework.Graphics.Texture2D>)
                                                            vars["Function: Redify"])
                                                        (StardewValley.Game1.content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>("Characters/Pierre"))
                                                    ))),
                                                    System.Linq.Expressions.Expression.Constant(StardewModdingAPI.Events.AssetLoadPriority.Medium),
                                                    System.Linq.Expressions.Expression.Constant(null)
                                                ),
                                                (new dynamic[] {
                                                @"
                                                This conditional checks if the asset being requested is Portraits/Dobson.
                                                If true: add the portraits.
                                                If false: continue down the tower.
                                                ",
                                                System.Linq.Expressions.Expression.Condition(
                                                    System.Linq.Expressions.Expression.Call(
                                                        typeof(StardewModdingAPI.IAssetName)
                                                            .GetMethod("IsEquivalentTo", new System.Type[]{typeof(string)}),
                                                        System.Linq.Expressions.Expression.Property(
                                                            ((System.Linq.Expressions.ParameterExpression)vars["LinqParam: e"]),
                                                            "NameWithoutLocale"
                                                        ),
                                                        System.Linq.Expressions.Expression.Constant("Portraits/Dobson")
                                                    ),
                                                    System.Linq.Expressions.Expression.Call(
                                                        ((System.Linq.Expressions.ParameterExpression)vars["LinqParam: e"]),
                                                        typeof(StardewModdingAPI.Events.AssetRequestedEventArgs)
                                                            .GetMethod(
                                                                "LoadFrom",
                                                                new System.Type[] {
                                                                    typeof(System.Func<object>),
                                                                    typeof(StardewModdingAPI.Events.AssetLoadPriority),
                                                                    typeof(string)
                                                                }
                                                            ),
                                                        System.Linq.Expressions.Expression.Constant(((System.Func<object>)(() =>
                                                            ((System.Func<Microsoft.Xna.Framework.Graphics.Texture2D, Microsoft.Xna.Framework.Graphics.Texture2D>)
                                                                vars["Function: Redify"])
                                                            (StardewValley.Game1.content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>("Portraits/Pierre"))
                                                        ))),
                                                        System.Linq.Expressions.Expression.Constant(StardewModdingAPI.Events.AssetLoadPriority.Medium),
                                                        System.Linq.Expressions.Expression.Constant(null)
                                                    ),
                                                    (new dynamic[] {
                                                    @"
                                                    This conditional checks if the asset being requested is Data/Kronosta/OneExpressionNPC/Redified.
                                                    If true: load the redified image.
                                                    If false: continue down the tower.
                                                    ",
                                                    System.Linq.Expressions.Expression.Condition(
                                                        System.Linq.Expressions.Expression.Call(
                                                            typeof(StardewModdingAPI.IAssetName)
                                                                .GetMethod("IsEquivalentTo", new System.Type[]{typeof(string)}),
                                                            System.Linq.Expressions.Expression.Property(
                                                                ((System.Linq.Expressions.ParameterExpression)vars["LinqParam: e"]),
                                                                "NameWithoutLocale"
                                                            ),
                                                            System.Linq.Expressions.Expression.Constant("Data/Kronosta/OneExpressionNPC/Redified")
                                                        ),
                                                        System.Linq.Expressions.Expression.Call(
                                                            ((System.Linq.Expressions.ParameterExpression)vars["LinqParam: e"]),
                                                            typeof(StardewModdingAPI.Events.AssetRequestedEventArgs)
                                                                .GetMethod(
                                                                    "Edit",
                                                                    new System.Type[] {
                                                                        typeof(System.Action<StardewModdingAPI.IAssetData>),
                                                                        typeof(StardewModdingAPI.Events.AssetEditPriority),
                                                                        typeof(string)
                                                                    }
                                                                ),
                                                            System.Linq.Expressions.Expression.Lambda<System.Action<StardewModdingAPI.IAssetData>>(
                                                                System.Linq.Expressions.Expression.Block(
                                                                    new System.Linq.Expressions.ParameterExpression[]
                                                                    {
                                                                        ((System.Linq.Expressions.ParameterExpression)vars["LinqParam: asset 3 data"]),
                                                                        ((System.Linq.Expressions.ParameterExpression)vars["LinqParam: asset 3 array"])
                                                                    },
                                                                    System.Linq.Expressions.Expression.Assign(
                                                                        ((System.Linq.Expressions.ParameterExpression)vars["LinqParam: asset 3 data"]),
                                                                        System.Linq.Expressions.Expression.Property(
                                                                            System.Linq.Expressions.Expression.Call(
                                                                                ((System.Linq.Expressions.ParameterExpression)vars["LinqParam: asset 3"]),
                                                                                typeof(StardewModdingAPI.IAssetData).GetMethod("AsImage")
                                                                            ),
                                                                            "Data"
                                                                        )
                                                                    ),
                                                                    System.Linq.Expressions.Expression.Assign(
                                                                        ((System.Linq.Expressions.ParameterExpression)vars["LinqParam: asset 3 array"]),
                                                                        System.Linq.Expressions.Expression.Call(
                                                                            typeof(System.Array)
                                                                                .GetMethod("CreateInstance", new System.Type[]{typeof(System.Type), typeof(int)}),
                                                                            System.Linq.Expressions.Expression.Constant(typeof(Microsoft.Xna.Framework.Color)),
                                                                            System.Linq.Expressions.Expression.Multiply(
                                                                                System.Linq.Expressions.Expression.Property(
                                                                                    ((System.Linq.Expressions.ParameterExpression)vars["LinqParam: asset 3 data"]),
                                                                                    "Width"
                                                                                ),
                                                                                System.Linq.Expressions.Expression.Property(
                                                                                    ((System.Linq.Expressions.ParameterExpression)vars["LinqParam: asset 3 data"]),
                                                                                    "Height"
                                                                                )
                                                                            )
                                                                        )
                                                                    ),
                                                                    System.Linq.Expressions.Expression.Call(
                                                                        ((System.Linq.Expressions.ParameterExpression)vars["LinqParam: asset 3 data"]),
                                                                        System.Linq.Enumerable.First(
                                                                            System.Linq.Enumerable.Where(
                                                                                typeof(Microsoft.Xna.Framework.Graphics.Texture2D).GetMethods(),
                                                                                (x => (x.Name == "GetData") && (x.GetParameters().Length == 1))
                                                                            )
                                                                        )
                                                                        .MakeGenericMethod(new System.Type[]{typeof(Microsoft.Xna.Framework.Color)}),
                                                                        ((System.Linq.Expressions.ParameterExpression)vars["LinqParam: asset 3 array"])
                                                                    ),
                                                                    System.Linq.Expressions.Expression.Call(
                                                                        ((System.Linq.Expressions.ParameterExpression)vars["LinqParam: asset 3 data"]),
                                                                        System.Linq.Enumerable.First(
                                                                            System.Linq.Enumerable.Where(
                                                                                typeof(Microsoft.Xna.Framework.Graphics.Texture2D).GetMethods(),
                                                                                (x => (x.Name == "SetData") && (x.GetParameters().Length == 1))
                                                                            )
                                                                        ),
                                                                        System.Linq.Expressions.Expression.Call(
                                                                            System.Linq.Enumerable.First(
                                                                                System.Linq.Enumerable.Where(
                                                                                    typeof(System.Linq.Enumerable).GetMethods(),
                                                                                    (x => x.Name == "Select" && (x.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == typeof(System.Func<,>)))
                                                                                )
                                                                            )
                                                                            .MakeGenericMethod(new System.Type[]{typeof(Microsoft.Xna.Framework.Color), typeof(Microsoft.Xna.Framework.Color)}),
                                                                            ((System.Linq.Expressions.ParameterExpression)vars["LinqParam: asset 3 array"]),
                                                                            System.Linq.Expressions.Expression.Constant(
                                                                                ((System.Func<Microsoft.Xna.Framework.Color, Microsoft.Xna.Framework.Color>)(c =>
                                                                                    new Microsoft.Xna.Framework.Color((byte)255, c.G, c.B, c.A)
                                                                                ))
                                                                            )
                                                                        )
                                                                    )
                                                                ),
                                                                false,
                                                                new System.Linq.Expressions.ParameterExpression[]
                                                                {
                                                                    ((System.Linq.Expressions.ParameterExpression)vars["LinqParam: asset 3"]),
                                                                }
                                                            ),
                                                            System.Linq.Expressions.Expression.Constant(StardewModdingAPI.Events.AssetEditPriority.Default),
                                                            System.Linq.Expressions.Expression.Constant(null)
                                                        ),
                                                        System.Linq.Expressions.Expression.Constant(0)
                                                    )}[1])
                                                )}[1])
                                            )}[1])
                                        )}[1])
                                    )}[1]),
                                    false,
                                    new System.Linq.Expressions.ParameterExpression[]
                                    {
                                        ((System.Linq.Expressions.ParameterExpression)vars["LinqParam: sender"]),
                                        ((System.Linq.Expressions.ParameterExpression)vars["LinqParam: e"]),
                                    }
                                ).Compile()
                                }
                            )
                        }
                    ))
                    (typeof(StardewModdingAPI.Events.IContentEvents)
                        .GetEvent("AssetRequested")
                    )

                }[0]
            ))
            (new System.Collections.Generic.Dictionary<string, object> 
            {
                ["Method: Monitor.Log"] = 
                    typeof(StardewModdingAPI.IMonitor)
                    .GetMethod("Log", new System.Type[] {typeof(string), typeof(StardewModdingAPI.LogLevel)}),
                ["State: Texture to redify"] = null,
                ["LinqParam: sender"] = System.Linq.Expressions.Expression.Parameter(typeof(object)),
                ["LinqParam: e"] = System.Linq.Expressions.Expression.Parameter(typeof(StardewModdingAPI.Events.AssetRequestedEventArgs)),
                ["LinqParam: asset 1"] = System.Linq.Expressions.Expression.Parameter(typeof(StardewModdingAPI.IAssetData)),
                ["LinqParam: asset 2"] = System.Linq.Expressions.Expression.Parameter(typeof(StardewModdingAPI.IAssetData)),
                ["LinqParam: asset 3"] = System.Linq.Expressions.Expression.Parameter(typeof(StardewModdingAPI.IAssetData)),
                ["LinqParam: asset 3 data"] = System.Linq.Expressions.Expression.Parameter(typeof(Microsoft.Xna.Framework.Graphics.Texture2D)),
                ["LinqParam: asset 3 array"] = System.Linq.Expressions.Expression.Parameter(typeof(Microsoft.Xna.Framework.Color)),
                ["LinqLabel: redify break inner loop"] = System.Linq.Expressions.Expression.Label(),
                ["LinqLabel: redify break outer loop"] = System.Linq.Expressions.Expression.Label()
            })
            ;
        }
    }
}
