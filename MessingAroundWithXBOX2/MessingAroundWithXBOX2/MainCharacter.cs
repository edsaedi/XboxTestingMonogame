using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MessingAroundwithXBOX
{
    public class MainCharacter : Sprite
    {
        public List<Projectile> projectiles = new List<Projectile>();
        public Rectangle Hitbox => Bounds;
        public int Points;
        public int Length => projectiles.Count;

        SoundEffect laserSound;
        SoundEffectInstance laserSoundInstance;

        //FireRate
        //FireTimer
        TimeSpan FireRate;
        TimeSpan fireTimer = TimeSpan.Zero;

        SpriteFont font;
        float deadzone = 0.15f;
        float bulletSpeed = 8;


        public MainCharacter(Texture2D texture, Vector2 position, TimeSpan fireRate, SoundEffect laserSound, SpriteFont font)
            : base(texture, position, Color.White, true)
        {
            this.laserSound = laserSound;
            this.FireRate = fireRate;
            this.font = font;
        }

        public void Update(Rectangle screen, Texture2D projectileImage, GameTime time, PlayerIndex index)
        {
            if (!screen.Contains(Hitbox))
            {
                Visible = false;
            }
            var state = GamePad.GetState(index);
            for (int i = 0; i < Length; i++)
            {
                projectiles[i].Update();

                if (!screen.Contains(projectiles[i].Hitbox))
                {
                    projectiles.RemoveAt(i);
                }
            }

            fireTimer += time.ElapsedGameTime;
            if (fireTimer >= FireRate && Visible && state.ThumbSticks.Right.Length() > deadzone)
            {
                laserSoundInstance = laserSound.CreateInstance();
                laserSoundInstance.Play();
                fireTimer = TimeSpan.Zero;

                Color color = Color.White;
                if (index == PlayerIndex.One)
                {
                    color = Color.Red;
                }
                else if (index == PlayerIndex.Two)
                {
                    color = Color.Blue;
                }
                else if (index == PlayerIndex.Three)
                {
                    color = Color.Yellow;
                }
                else
                {
                    color = Color.Green;
                }

                Projectile shot = new Projectile(projectileImage, Position, new Vector2(10, 0), color);
                var stick = new Vector2(state.ThumbSticks.Right.X, -state.ThumbSticks.Right.Y);
                stick.Normalize();

                shot.Speed = stick * bulletSpeed;

                //calculate angle of joystick
                double angle = Math.Atan2(stick.Y, stick.X);

                projectiles.Add(shot);
                projectiles.Last().Rotation = (float)angle;
            }

            if (state.DPad.Up == ButtonState.Pressed)
            {
                Y -= 2;
            }
            else if (state.DPad.Down == ButtonState.Pressed)
            {
                Y += 2;
            }
            else
            {
                Y += state.ThumbSticks.Left.Y * -10;
            }

            if (state.DPad.Left == ButtonState.Pressed)
            {
                X -= 2;
            }
            else if (state.DPad.Right == ButtonState.Pressed)
            {
                X += 2;
            }
            else
            {
                X += state.ThumbSticks.Left.X * 10;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle screen)
        {
            if (Visible)
            {
                base.Draw(spriteBatch);
                foreach (var laser in projectiles)
                {
                    laser.Draw(spriteBatch);
                }
            }
        }

        /*public void multiplayerUpdate(MainCharacter character)
        {
            for (int i = 0; i < Length; i++)
            {
                if (projectiles[i].Hitbox.Contains(character.Hitbox))
                {
                    character.Visible = false;
                }
            }
        }*/
    }
}
