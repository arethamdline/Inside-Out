import os
import json
from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
from openai import AsyncOpenAI  # ✅ Async client
from dotenv import load_dotenv

load_dotenv()

client = AsyncOpenAI(
  api_key=os.getenv("OPENAPI_KEY")
)

app = FastAPI()
history: dict[str, list] = {}

characters = {
    "Anger": "You are an honest, helpful but straightforward character.",
    "Disgust": "You are an excitable and curious character who asks many questions.",
    "Joy": "You are a wise character who speaks carefully.",
    "Fear": "You are an intimidating but honest character.",   # ✅ Added comma
    "Sadness": "You are empathetic but nostalgic."
}

class UserInput(BaseModel):
    character: str
    text_message: str

class CharacterResponse(BaseModel):
    text_message: str

@app.post("/receive_input/", response_model=CharacterResponse)
async def receive_input(user_input: UserInput):
    try:
        print("received input: " + user_input.text_message)
        response_json = await call_ai(
            prompt=user_input.text_message,
            character=user_input.character
        )
        return CharacterResponse(text_message=response_json["text"])
    except Exception as e:
        print(e)
        raise HTTPException(status_code=500, detail="AI processing failed")

async def call_ai(prompt: str, character: str) -> dict:
    if character not in history:
        history[character] = [{
            "role": "system",
            "content": characters.get(
                character,
                "You are a helpful empathetic conversational character giving advice."
            ) + ' Respond only in JSON format with fields: { "text": string }'
        }]

    history[character].append({"role": "user", "content": prompt})

    response = await client.responses.create(
        model="gpt-5-nano",
        input=history[character]
    )

    ai_content = response.output_text

    history[character].append({"role": "assistant", "content": ai_content})

    return json.loads(ai_content)