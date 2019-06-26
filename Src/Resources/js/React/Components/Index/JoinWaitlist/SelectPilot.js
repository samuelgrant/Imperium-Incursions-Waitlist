import React, { Component } from 'react';

export default class SelectPilot extends Component {
    constructor(props) {
        super(props);

        this.state = {
            initialPilotSet: false
        }
    }

    componentDidUpdate() {
        if (this.state.initialPilotSet) return;

        this.props.selectedPilot(this.props.prefPilot.id);
        this.setState({ initialPilotSet: true });
    }

    updateSelectedPilot(e) {
        this.props.selectedPilot(e.target.value);
    }

    

    render() {
        let pilotOptions;
        if (this.props.pilots != null) {
            pilotOptions = this.props.pilots.map((pilot) => {
                let selected = false;
                if (this.props.prefPilot && pilot.id == this.props.prefPilot.id) selected = true;

                return <option value={pilot.id} selected={selected}>{pilot.name}</option>
            });
        }

        return (
            <div>
                <label htmlFor="selectPilot">Waitlist With:</label>
                <select id="selectPilot" className="form-control" onChange={this.updateSelectedPilot.bind(this)} >
                    {pilotOptions}
                </select>
            </div>
        )
    }
}