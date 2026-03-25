import os
import json
from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
from openai import AsyncOpenAI  
from dotenv import load_dotenv

load_dotenv()

client = AsyncOpenAI(
  api_key=os.getenv("OPENAPI_KEY")
)

app = FastAPI()
history: dict[str, list] = {}

characters = {
    "Anger": "You are an honest, helpful but straightforward character.",
    "Disgust": "You are a hepful and critical but sarcastic character. Give a 'yikes' response",
    "Joy": "You are an optimistic and energetic character.",
    "Fear": "You prioritises safety, and anticipative; show anxiousness and give a stuttered response",   
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
            ) + ' Respond only in JSON format with fields: { "text": string }' + ' Keep response short and conversational (1-2 sentences)'
        }]

    history[character].append({"role": "user", "content": prompt})

    response = await client.responses.create(
        model="gpt-5-nano",
        input=history[character]
    )

    ai_content = response.output_text

    history[character].append({"role": "assistant", "content": ai_content})

    return json.loads(ai_content)